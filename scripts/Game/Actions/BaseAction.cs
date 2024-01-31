using System;
using System.Linq.Expressions;
using DSS.Game.DuckTyping;
using Godot;

namespace DSS.Game.Actions;

public abstract class BaseAction
{
    protected Guid Id;
    protected Game GameRef;
    public Entity EntityRef;
    public Action OnSuccess;
    public Action OnFailure;
    public bool? Result;
    
    protected BaseAction(Game game, Entity entity, Action onSuccess = null, Action onFailure = null)
    {
        Id = Guid.NewGuid();
        GameRef = game;
        EntityRef = entity;
        OnSuccess = onSuccess;
        OnFailure = onFailure;
        Result = null;
    }
    
    public bool Perform()
    {
        if (!GameRef.ActionManager.HasRegistered(this))
        {
            GameRef.ActionManager.Register(this);
        }
        Result = TryPerform();
        if (Result.HasValue)
        {
            if (Result.Value)
            {
                OnSuccess?.Invoke();
            }
            else
            {
                OnFailure?.Invoke();
            }

            return Result.Value;
        }
        else
        {
            GD.PrintErr(this, "Perform failed");
            return false;
        }
    }
    
    public abstract bool TryPerform();
}