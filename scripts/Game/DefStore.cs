using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using DSS.Common;
using DSS.Defs;
using Godot;

namespace DSS.Game;

public class DefStore
{
    public Dictionary<Type, Dictionary<DefId, BaseDef>> Defs = new ();
    protected Dictionary<string, int> TileIdMap = new (); 

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
                { GetTileId("hidden"), new Vector2I(0, 0) },
                { GetTileId("revealed"), new Vector2I(1, 0) },
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
                Id = 0,
                StrId = "floor",
                DisplayName = "Floor",
                Description = "Floor",
                Type = Enums.TileType.Terrain,
            },
            new TileDef()
            {
                Id = 1,
                StrId = "wall",
                DisplayName = "Wall",
                Description = "Wall",
                Type = Enums.TileType.Terrain,
                Flags = new [] { Enums.TileFlag.Wall, Enums.TileFlag.Occlusion }
            },
            new TileDef()
            {
                Id = 2,
                StrId = "player",
                DisplayName = "Player",
                Description = "Player",
                Type = Enums.TileType.Entity,
            },
            new TileDef()
            {
                Id = 3,
                StrId = "adventurer",
                DisplayName = "Adventurer",
                Description = "Adventurer",
                Type = Enums.TileType.Entity,
            },
            new TileDef()
            {
                Id = 4,
                StrId = "hidden",
                DisplayName = "Hidden",
                Description = "Hidden",
                Type = Enums.TileType.VisibilityOverlay,
            },
            new TileDef()
            {
                Id = 5,
                StrId = "revealed",
                DisplayName = "Revealed",
                Description = "Revealed",
                Type = Enums.TileType.VisibilityOverlay,
            },
        };
        foreach (var tileDef in tileDefs)
        {
            AddDef(tileDef);
            TileIdMap.Add(tileDef.StrId, tileDef.Id);
        }
    }

}