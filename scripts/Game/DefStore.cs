using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using DSS.Common;
using DSS.Defs;
using Godot;

namespace DSS.Game;

public class DefStore
{
    public Dictionary<Type, Dictionary<DefId, BaseDef>> Defs = new();
    protected Dictionary<string, int> TileIdMap = new();
    protected Dictionary<int, string> TileStrIdMap = new();

    public void Init()
    {
        // MOCK:
        LoadTiles();
        var tileAtlasDef = new TileAtlasDef
        {
            Path = Constants.MonoTileSetDefPath,
            StrId = Constants.MonoTileSetDefStrId,
            AtlasPath = Constants.MonoTileSetAtlasPath,
            TileIdToCoordMap = new Dictionary<int, Vector2I>
            {
                { GetTileId("floor"), new Vector2I(0, 0) },
                { GetTileId("wall"), new Vector2I(2, 0) },
                { GetTileId("player"), new Vector2I(3, 7) },
                { GetTileId("adventurer"), new Vector2I(6, 1)}
            },
            TileSize = new Vector2I(8, 8)
        };
        AddDef(tileAtlasDef);
        tileAtlasDef = new TileAtlasDef()
        {
            Path = Constants.TilemapVisibilityOverlayDefPath,
            StrId = Constants.TilemapVisibilityOverlayDefStrId,
            AtlasPath = Constants.TilemapVisibilityOverlayAtlasPath,
            TileIdToCoordMap = new Dictionary<int, Vector2I>()
            {
                { GetTileId("covered"), new Vector2I(0, 0) },
                { GetTileId("explored"), new Vector2I(1, 0) },
            },
            TileSize = new Vector2I(8, 8)
        };
        AddDef(tileAtlasDef);
    }
    
    public T GetDef<T>(DefId defId) where T : BaseDef
    {
        var type = typeof(T);
        if (Defs.TryGetValue(type, out var defs))
        {
            if (defs.TryGetValue(defId, out var def))
            {
                return def as T;
            }
        }
        return null;
    }

    public int GetTileId(string strTileId)
    {
        if (TileIdMap.TryGetValue(strTileId, out var tileId))
        {
            return tileId;
        }
        return 0;
    }

    public TileDef GetTileDef(int tileId) => GetDef<TileDef>(new DefId
        { FilePath = Constants.TileDefPath, InFileStrId = TileStrIdMap[tileId] });

    protected void AddDef<T>(T def) where T : BaseDef
    {
        var type = typeof(T);
        if (!Defs.ContainsKey(type))
        {
            Defs.Add(type, new Dictionary<DefId, BaseDef>());
        }
        Defs[type].Add(new DefId() { FilePath = def.Path, InFileStrId = def.StrId }, def);
    }
    
    protected void LoadTiles()
    {
        // MOCK:
        var tileDefs = new []
        {
            new TileDef()
            {
                Path = Constants.TileDefPath,
                Id = 0,
                Type = Enums.TileType.Terrain,
                StrId = "floor",
                DisplayName = "Floor",
                Description = "Floor",
            },
            new TileDef()
            {
                Path = Constants.TileDefPath,
                Id = 1,
                Type = Enums.TileType.Wall,
                StrId = "wall",
                DisplayName = "Wall",
                Description = "Wall",
                Flags = new [] { Enums.EntityFlag.Occlusion }
            },
            new TileDef()
            {
                Path = Constants.TileDefPath,
                Id = 2,
                Type = Enums.TileType.Creature,
                StrId = "player",
                DisplayName = "Player",
                Description = "Player",
            },
            new TileDef()
            {
                Path = Constants.TileDefPath,
                Id = 3,
                Type = Enums.TileType.Creature,
                StrId = "adventurer",
                DisplayName = "Adventurer",
                Description = "Adventurer",
            },
            new TileDef()
            {
                Path = Constants.TileDefPath,
                Id = 4,
                Type = Enums.TileType.Overlay,
                StrId = "covered",
                DisplayName = "Covered",
                Description = "Covered",
            },
            new TileDef()
            {
                Path = Constants.TileDefPath,
                Id = 5,
                Type = Enums.TileType.Overlay,
                StrId = "explored",
                DisplayName = "Explored",
                Description = "Explored",
            },
        };
        foreach (var tileDef in tileDefs)
        {
            AddDef(tileDef);
            TileIdMap.Add(tileDef.StrId, tileDef.Id);
            TileStrIdMap.Add(tileDef.Id, tileDef.StrId);
        }
    }

}