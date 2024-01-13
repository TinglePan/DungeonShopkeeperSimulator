using System;
using System.Collections.Generic;
using DSS.Game.Actions;
using Godot;

namespace DSS.Game;

public abstract class BaseGameHandler
{
    public Game GameRef;
    
    public virtual void OnEnter()
    {
        
    }

    public virtual void OnExit()
    {
        
    }

    protected BaseGameHandler(Game game)
    {
        GameRef = game;
    }
    
    public void TransitionTo(BaseGameHandler handler)
    {
        GameRef.GameState.CurrentGameHandler.OnExit();
        GameRef.GameState.CurrentGameHandler = handler;
        handler.OnEnter();
    }
}