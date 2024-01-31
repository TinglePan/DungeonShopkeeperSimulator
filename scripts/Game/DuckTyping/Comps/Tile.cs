namespace DSS.Game.DuckTyping.Comps;

public class Tile: BaseComp
{
    public int TileId;
    
    public static void Setup(Game game, Entity obj, int tileId)
    {
        var tileComp = obj.GetCompOrNew<Tile>();
        tileComp.GameRef = game;
        tileComp.EntityRef = obj;
        tileComp.TileId = tileId;
    }
}