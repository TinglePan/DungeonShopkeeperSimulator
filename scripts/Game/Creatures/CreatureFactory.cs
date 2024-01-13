using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game;

public static class CreatureFactory
{
    public static DuckObject CreatePlayer(DefStore defStore)
    {
        var player = new DuckObject();
        TileRenderable.Setup(player, Constants.MonoTileSetAtlasPath, defStore.GetTileId("player"));
        return player;
    }

    public static DuckObject CreateAdventurer(DefStore defStore)
    {
        var adventurer = new DuckObject();
        TileRenderable.Setup(adventurer, Constants.MonoTileSetAtlasPath, defStore.GetTileId("adventurer"));
        return adventurer;
    }
}