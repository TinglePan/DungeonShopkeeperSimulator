using System.Dynamic;
using System.IO;
using DSS.Common;
using DSS.Defs;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.DuckTyping;

public static class EntityFactory
{
    private static Entity CreateTileEntity(Game game, string atlasPath, int tileId, bool hideUnseen)
    {
        var entity = new Entity();
        var defPath = Utils.TexturePathToDefPath(atlasPath);
        var tileSetAtlasDefId = new DefId() { FilePath = defPath, InFileStrId = Path.GetFileNameWithoutExtension(defPath) };
        var tileSetAtlasDef = game.DefStore.GetDef<TileAtlasDef>(tileSetAtlasDefId);
        var tileRenderableOffset = tileSetAtlasDef.GetTileCoord(tileId);
        TileRenderable.Setup(game, entity, atlasPath, tileRenderableOffset, hideUnseen);
        Tile.Setup(game, entity, tileId);
        return entity;
    }

    public static Entity CreatePlayer(Game game, string atlasPath)
    {
        var res = CreateTileEntity(game, atlasPath, game.DefStore.GetTileId("player"), false);
        Collider.Setup(game, res, new[] { Enums.CollisionLayer.FriendlyCreature }, new[]
        {
            Enums.CollisionLayer.Wall,
            Enums.CollisionLayer.Furniture,
            Enums.CollisionLayer.Water,
        });
        UniversalFlags.Setup(game, res, Enums.EntityFlag.Dynamic);
        return res;
    }

    public static Entity CreateAdventurer(Game game, string atlasPath)
    {
        var res = CreateTileEntity(game, atlasPath, game.DefStore.GetTileId("adventurer"), true);
        res.GetComp<TileRenderable>().HideUnseen = true;
        Collider.Setup(game, res, new[] { Enums.CollisionLayer.HostileCreature }, new[]
        {
            Enums.CollisionLayer.Wall,
            Enums.CollisionLayer.Furniture,
            Enums.CollisionLayer.Water,
        });
        UniversalFlags.Setup(game, res, Enums.EntityFlag.Dynamic);
        return res;
    }

    public static Entity CreateWall(Game game, string atlasPath)
    {
        var res = CreateTileEntity(game, atlasPath, game.DefStore.GetTileId("wall"), false);
        Collider.Setup(game, res, new []{ Enums.CollisionLayer.Wall }, new[]
        {
            Enums.CollisionLayer.FriendlyCreature,
            Enums.CollisionLayer.HostileCreature,
            Enums.CollisionLayer.Furniture,
            Enums.CollisionLayer.Water,
        });
        UniversalFlags.Setup(game, res, Enums.EntityFlag.Occlusion);
        return res;
    }
}