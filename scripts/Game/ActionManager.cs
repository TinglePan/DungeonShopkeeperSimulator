using System;
using System.Collections.Generic;
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
    
    public bool Perform(BaseAction action, Action onSuccess=null, Action onFailure=null)
    {
        action.OnSuccess = onSuccess;
        action.OnFailure = onFailure;
        action.Perform();
        RecordHistory(action);
        return action.Result;
    }
    
    protected void RecordHistory(BaseAction action)
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