using System;
using DSS.Common;
using DSS.Game.DuckTyping;

namespace DSS.Game.Actions;

public abstract class BaseDirectionalAction: BaseAction, IDirectionalAction
{
    public Enums.Direction8 Dir { get; set; }
    
    public BaseDirectionalAction(Game game, Entity entity, Enums.Direction8 dir, Action onSuccess = null, Action onFailure = null): base(game, entity, onSuccess, onFailure)
    {
        Dir = dir;
    }
}