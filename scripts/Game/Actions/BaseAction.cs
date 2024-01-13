using System;
using System.Linq.Expressions;

namespace DSS.Game.Actions;

public abstract class BaseAction
{
    public Action OnSuccess;
    public Action OnFailure;
    public bool Result;
    
    public void Perform()
    {
        Result = TryPerform();
        if (Result)
        {
            OnSuccess?.Invoke();
        }
        else
        {
            OnFailure?.Invoke();
        }
    }
    
    protected abstract bool TryPerform();
}