using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Godot;
using SadRogue.Primitives;

namespace DSS.Common;

public static class Utils
{
    public static IEnumerable<Vector2I> GetLine(Vector2I start, Vector2I end)
    {
        foreach (var point in Lines.GetBresenhamLine(start.X, start.Y, end.X, end.Y))
        {
            yield return new Vector2I(point.X, point.Y);
        }
    }

    public static IEnumerable<Vector2I> GetNeighbours8(Vector2I coord)
    {
        
        for (var x = coord.X - 1; x <= coord.X + 1; x++)
        {
            for (var y = coord.Y - 1; y <= coord.Y + 1; y++)
            {
                if (x == coord.X && y == coord.Y)
                {
                    continue;
                }
                yield return new Vector2I(x, y);
            }
        }
    }
    
    public static string TexturePathToDefPath(string texturePath)
    {
        return $"res://defs/{Path.GetFileNameWithoutExtension(texturePath.GetBaseName())}.json";
    }

    public static int CountIdenticalBits(int a, int b, int maxBits)
    {
        var xorRes = a ^ b;
        var count = 0;
        while (maxBits > 0)
        {
            count += xorRes & 1;
            xorRes >>= 1;
            maxBits -= 1;
        }
        return count;
    }
}