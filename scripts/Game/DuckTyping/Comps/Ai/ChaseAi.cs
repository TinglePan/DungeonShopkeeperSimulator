using System.Collections.Generic;
using System.Linq;
using DSS.Common;
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
    
    protected Entity Target;
    protected Queue<Vector2I> Path = new ();
    protected bool IsOutdated = true;
    protected State CurrentState = State.Idle;

    public static void Setup(Game game, Entity obj, Entity target)
    {
        var aiComp = obj.GetCompOrNew<ChaseAi>();
        BaseAi.Setup(game, obj);
        aiComp.Target = target;
        // NOTE: need to postpone TryChase() until map info is updated
        target.GetComp<OnMap>().Coord.OnChanged += (_, _) => aiComp.IsOutdated = true;
    }

    public override void Step(ActionManager actionManager)
    {
        switch (CurrentState)
        {
            case State.Idle:
                TryChase();
                break;
            case State.Chasing:
                var entityOnMapComp = EntityRef.GetComp<OnMap>();
                var targetOnMapComp = Target.GetComp<OnMap>();
                if (!entityOnMapComp.Map.Equals(targetOnMapComp.Map))
                {
                    CurrentState = State.Idle;
                    return;
                }
                if (entityOnMapComp.Coord.Value == targetOnMapComp.Coord.Value)
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
                    bool res = new BumpAction(GameRef, EntityRef,
                        (Enums.Direction8)Utils.DirBetweenCoords(entityOnMapComp.Coord.Value, nextCoord)).Perform();
                    if (!res)
                    {
                        CurrentState = State.Idle;
                    }
                }
                break;
        }
    }

    public void TryChase()
    {
        Path.Clear();
        var entityOnMapComp = EntityRef.GetComp<OnMap>();
        var targetOnMapComp = Target.GetComp<OnMap>();
        var map = entityOnMapComp.Map;
        var path = map.Navigate(entityOnMapComp.Coord.Value, targetOnMapComp.Coord.Value).ToArray();
        if (path.Count() > 2)
        {
            foreach (var wayPoint in path.Skip(1).SkipLast(1))
            {
                Path.Enqueue(wayPoint);
            }
            CurrentState = State.Chasing;
        }
    }
}