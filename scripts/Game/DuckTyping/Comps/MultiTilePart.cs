using DSS.Game.Actions;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class MultiTilePart: BaseComp
{
    public Entity ParentRef;
    public Vector2I Offset;

    public static void Setup(Game game, Entity obj, Entity parent, Vector2I offset)
    {
        var multiTilePartComp = obj.GetCompOrNew<MultiTilePart>();
        multiTilePartComp.GameRef = game;
        multiTilePartComp.EntityRef = obj;
        multiTilePartComp.ParentRef = parent;
        multiTilePartComp.Offset = offset;
    }

}