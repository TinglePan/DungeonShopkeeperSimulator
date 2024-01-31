using System.Collections.Generic;
using DSS.Common;
using Godot;

namespace DSS.Defs;

public class TileAtlasDef: BaseDef
{
    public string AtlasPath;
    public Dictionary<int, Vector2I> TileIdToCoordMap;
    public Vector2I TileSize;
    
    public Vector2I GetTileCoord(int tileId)
    {
        return TileIdToCoordMap[tileId];
    }
}