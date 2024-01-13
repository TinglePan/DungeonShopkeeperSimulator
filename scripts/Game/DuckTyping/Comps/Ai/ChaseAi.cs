﻿using System.Collections.Generic;
using System.Linq;
using DSS.Game.Actions;
using Godot;

namespace DSS.Game.DuckTyping.Comps.Ai;


public class ChaseAi: BaseAi
{
    public enum State 
    {
        Idle,
        Chasing,
    }
    
    protected DuckObject Target;
    protected Queue<Vector2I> Path = new Queue<Vector2I>();
    protected bool IsOutdated = true;
    protected State CurrentState = State.Idle;

    public static void Setup(DuckObject obj, DuckObject target)
    {
        BaseAi.Setup(obj);
        var aiComp = obj.GetCompOrNew<ChaseAi>();
        aiComp.Target = target;
        // OnMap.WatchCoordChange(target, (_, _) => aiComp.TryChase());
        // NOTE: need to postpone TryChase() until map info is updated
        OnMap.WatchCoordChange(target, (_, _) => aiComp.IsOutdated = true);
    }

    public override void Step(ActionManager actionManager)
    {
        switch (CurrentState)
        {
            case State.Idle:
                TryChase();
                break;
            case State.Chasing:
                if (!OnMap.IsOnSameMap(CreatureRef, Target))
                {
                    CurrentState = State.Idle;
                    return;
                }
                var targetCoord = OnMap.GetCoord(Target);
                if (OnMap.GetCoord(CreatureRef) == targetCoord)
                {
                    CurrentState = State.Idle;
                    return;
                }
                // TODO: change to action point based
                if (IsOutdated || Path.Count == 0)
                {
                    TryChase();
                    IsOutdated = false;
                }
                if (Path.Count > 0)
                {
                    var nextCoord = Path.Dequeue();
                    var action = new MoveAction(CreatureRef, nextCoord);
                    actionManager.Perform(action, onFailure:() => CurrentState = State.Idle);
                }
                break;
        }
    }

    public void TryChase()
    {
        CalculatePathTo(OnMap.GetCoord(Target));
        CurrentState = State.Chasing;
    }

    protected void CalculatePathTo(Vector2I coord)
    {
        Path.Clear();
        var pathFinder = PathFinder.GetPathFinderFor(CreatureRef);
        foreach (var wayPoint in pathFinder.FindPath(OnMap.GetCoord(CreatureRef), coord).Skip(1).SkipLast(1))
        {
            Path.Enqueue(wayPoint);
        }
    }
}