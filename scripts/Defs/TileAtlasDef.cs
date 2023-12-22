using System.Collections.Generic;
using DSS.Common;
using Godot;

namespace DSS.Defs;

public class TileAtlasDef: BaseDef
{
    public string AtlasPath;
    public Dictionary<Enums.TileId, Vector2I> TileIdToCoordMap;
}