using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace DSS.Game.Components;

public record MultiTile
{
    public int Width;
    public int Height;
    public UInt16[] Tiles;
    
    public MultiTile(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new UInt16[width * height];
    }
    
    public MultiTile(int width, int height, UInt16[] tiles)
    {
        Width = width;
        Height = height;
        Tiles = tiles;
    }

    public MultiTile(int width, int height, Dictionary<Vector2I, UInt16> sparseTiles)
    {
        Width = width;
        Height = height;
        Tiles = new UInt16[width * height];
        foreach (var (coord, tile) in sparseTiles)
        {
            Debug.Assert(coord.X >= 0 && coord.X < width);
            Debug.Assert(coord.Y >= 0 && coord.Y < height);
            Tiles[coord.X + coord.Y * width] = tile;
        }
    }
}