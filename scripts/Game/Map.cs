using System;
using DSS.Common;
using DSS.Game.Components;
using Godot;
using SadRogue.Primitives.GridViews;

namespace DSS.Game;

public class Map
{
    public MultiTile MultiTile;
    public ArrayView<UInt16> View;
    public int Size => MultiTile.Width * MultiTile.Height;
    public Map(Vector2I dimension)
    {
        MultiTile = new MultiTile(dimension.X, dimension.Y);
        View = new ArrayView<UInt16>(MultiTile.Tiles, MultiTile.Width);
    }
    
    public Vector2I IndexToCoord(int index)
    {
        return new Vector2I(index % MultiTile.Width, index / MultiTile.Width); 
    }
    
    public int CoordToIndex(Vector2I coord)
    {
        return coord.X + coord.Y * MultiTile.Width;
    }
}