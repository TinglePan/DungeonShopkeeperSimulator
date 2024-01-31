using System;
using DSS.Game.DuckTyping;

namespace DSS.Game.Actions;

public class SwapPosAction: BaseAction
{
    public Entity Other;
    
    public SwapPosAction(Game game, Entity entity, Entity other, Action onSuccess = null, Action onFailure = null) : base(game, entity, onSuccess, onFailure)
    {
        Other = other;
    }

    public override bool TryPerform()
    {
        throw new NotImplementedException();
    }
}