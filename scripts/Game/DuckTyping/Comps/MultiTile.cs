using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class MultiTile: BaseComp
{
    public HashSet<Vector2I> Region;
    public Vector2I Pivot;
    public Dictionary<Vector2I, Entity> Tiles = new();
    
    public static void Setup(Game game, Entity obj, IEnumerable<Vector2I> region, Vector2I pivot)
    {
        var multiTileComp = obj.GetCompOrNew<MultiTile>();
        multiTileComp.GameRef = game;
        multiTileComp.EntityRef = obj;
        multiTileComp.Region = region.ToHashSet();
        multiTileComp.Pivot = pivot;
        foreach (var coord in multiTileComp.Region)
        {
            var part = new Entity();
            MultiTilePart.Setup(game, part, obj, coord);
            multiTileComp.Tiles.Add(coord, part);
        }
    }
}

public class RectangleMultiTile: MultiTile
{
    public Vector4I BoundingBox;
    
    public int Left => BoundingBox.X;
    public int Top => BoundingBox.Y;
    public int Right => Left + Width - 1;
    public int Bottom => Top + Height - 1;
    public int Width => BoundingBox.Z;
    public int Height => BoundingBox.W;
    
    public static void Setup(Game game, Entity obj, int width, int height, Vector2I pivot)
    {
        var multiTileComp = obj.GetCompOrNew<RectangleMultiTile>();
        var top = -pivot.Y;
        var left = -pivot.X;
        multiTileComp.BoundingBox = new Vector4I(left, top, width, height);
        MultiTile.Setup(game, obj, Utils.IterateBoundingBox(multiTileComp.BoundingBox), pivot);
    }
}