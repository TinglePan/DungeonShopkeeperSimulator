using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using DSS.Game.Actions;

namespace DSS.Game;

public class ActionManager
{
    protected Game GameRef;
    
    protected Queue<int> RecordedTurnIds = new ();
    protected Dictionary<int, Queue<BaseAction>> ActionHistory = new ();
    
    public void Init(Game game)
    {
        GameRef = game;
    }

    public bool HasRegistered(BaseAction action)
    {
        if (!ActionHistory.ContainsKey(GameRef.GameState.CurrentTurn)) return false;
        return ActionHistory[GameRef.GameState.CurrentTurn]?.Contains(action) ?? false;
    }
    
    public void Register(BaseAction action)
    {
        var currentTurn = GameRef.GameState.CurrentTurn;
        if (!RecordedTurnIds.Contains(currentTurn))
        {
            RecordedTurnIds.Enqueue(currentTurn);
            ActionHistory[currentTurn] = new Queue<BaseAction>();
        }
        ActionHistory[currentTurn].Enqueue(action);
    }
}