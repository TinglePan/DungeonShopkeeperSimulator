using System.Collections.Generic;
using DSS.Common;
using Godot;

namespace DSS.Defs;

public class TileAtlasDef: BaseDef
{
    public string AtlasPath;
    public Dictionary<int, Vector2I> TileIdToCoordMap;
    public Vector2I TileSize;
}