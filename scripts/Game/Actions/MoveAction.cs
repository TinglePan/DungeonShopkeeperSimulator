using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.Actions;

public class MoveAction: BaseDirectionalAction
{
    
    public MoveAction(Game game, Entity entity, Enums.Direction8 dir): base(game, entity, dir)
    {
    }
    
    public override bool TryPerform()
    {
        var map = EntityRef.GetComp<OnMap>().Map;
        map.MoveObject(EntityRef, Dir);
        return true;
    }
}