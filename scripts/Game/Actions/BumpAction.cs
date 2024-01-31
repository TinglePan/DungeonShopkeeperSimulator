using System.Reflection.PortableExecutable;
using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.Actions;

public class BumpAction: BaseDirectionalAction
{
    public BumpAction(Game game, Entity entity, Enums.Direction8 dir): base(game, entity, dir)
    {
        Dir = dir;
    }

    public override bool TryPerform()
    {
        var onMapComp = EntityRef.GetComp<OnMap>();
        var map = onMapComp.Map;
        var toCoord = onMapComp.Coord.Value + Utils.DirToDxy((Enums.Direction9)Dir);
        if (!map.IsInBounds(toCoord))
        {
            return false;
        }

        if (!(EntityRef.GetComp<Collider>(ensure: false) is {} entityColliderComp)) return new MoveAction(GameRef, EntityRef, Dir).Perform();

        foreach (var maybeObstacle in map.GetObjectsAt(toCoord, entity => entity.HasComp<Collider>()))
        {
            var obstacleColliderComp = maybeObstacle.GetComp<Collider>();
            if (entityColliderComp.InteractWith(Collider.MaskType.SwapPos, obstacleColliderComp))
            {
                return new SwapPosAction(GameRef, EntityRef, maybeObstacle).Perform();
            }
            if (entityColliderComp.InteractWith(Collider.MaskType.Melee, obstacleColliderComp))
            {
                return new MeleeAction(GameRef, EntityRef, Dir).Perform();
            }
            if (entityColliderComp.InteractWith(Collider.MaskType.Collision, obstacleColliderComp))
            {
                return false;
            }
        }
        return new MoveAction(GameRef, EntityRef, Dir).Perform();
    }
    
}