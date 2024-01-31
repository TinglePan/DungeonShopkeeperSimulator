using DSS.Common;
using DSS.Game.DuckTyping;

namespace DSS.Game.Actions;

public class TurnAroundAction: BaseDirectionalAction
{
    public TurnAroundAction(Game game, Entity entity, Enums.Direction8 dir): base(game, entity, dir)
    {
    }
    public override bool TryPerform()
    {
        throw new System.NotImplementedException();
    }
}