using System;
using System.Collections.Generic;
using DSS.Common;
using DSS.Defs;
using Godot;

namespace DSS.Game;

public class DefStore
{
    private static DefStore _instance;
    public static DefStore Instance => _instance ??= new DefStore();

    public Dictionary<Type, Dictionary<string, BaseDef>> Defs = new();

    public void Init()
    {
        var tileAtlasDef = new TileAtlasDef
        {
            Path = Constants.MonoTileSetDefPath,
            AtlasPath = Constants.MonoTileSetAtlasPath,
            TileIdToCoordMap = new Dictionary<Enums.TileId, Vector2I>
            {
                { Enums.TileId.Wall, new Vector2I(2, 0) },
            }
        };
        var type = tileAtlasDef.GetType();
        if (!Defs.ContainsKey(type))
        {
            Defs.Add(type, new Dictionary<string, BaseDef>());
        }
        Defs[type].Add(tileAtlasDef.Path, tileAtlasDef);
    }
    
    public T GetDef<T>(string path) where T : BaseDef
    {
        var type = typeof(T);
        if (Defs.TryGetValue(type, out var defs))
        {
            if (defs.TryGetValue(path, out var def))
            {
                return def as T;
            }
        }
        return null;
    }

}