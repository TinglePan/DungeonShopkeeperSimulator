using System;
using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.Actions;

public class MeleeAction: BaseDirectionalAction
{
    public Entity TargetRef;
    
    public MeleeAction(Game game, Entity entity, Enums.Direction8 dir, Action onSuccess = null, Action onFailure = null): base(game, entity, dir, onSuccess, onFailure)
    {
        Dir = dir;
    }

    public override bool TryPerform()
    {
        return false;
    }
}