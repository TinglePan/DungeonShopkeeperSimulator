using System;
using System.Collections.Generic;
using DSS.Game.Actions;
using Godot;

namespace DSS.Game;

public abstract class BaseGameHandler
{

    public Game GameRef;
    public abstract IEnumerable<BaseAction> HandleInput(InputEvent @event);
    public abstract BaseGameHandler Step(InputEvent @event);

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
}