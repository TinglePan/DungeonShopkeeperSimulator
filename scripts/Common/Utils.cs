using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Godot;
using SadRogue.Primitives;
using Vector2 = Godot.Vector2;

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

    public static IEnumerable<Vector2I> GetCurve(Vector2I coord, Vector2I pivot, float angle, bool clockwise=true)
    {
        angle = NormalizeAngle(angle);
        var radius = (coord - pivot).Length();
        var endPoint = RotateCoord(coord, pivot, angle);
        var candidates = new Queue<Vector2I>();
        candidates.Enqueue(coord);
        while (candidates.Count > 0)
        {
            var candidate = candidates.Dequeue();
            yield return candidate;
            if (candidate == endPoint)
            {
                yield break;
            };
            var foundCandidate = false;
            foreach(var neighbour in GetNeighbours4(candidate))
            {
                if (IsOnCurve(neighbour, pivot, radius, angle, clockwise))
                {
                    candidates.Enqueue(neighbour);
                    foundCandidate = true;
                    break;
                }
            }
            if (!foundCandidate)
            {
                foreach(var neighbour in GetNeighbours4(candidate, true))
                {
                    if (IsOnCurve(neighbour, pivot, radius, angle, clockwise))
                    {
                        candidates.Enqueue(neighbour);
                        foundCandidate = true;
                        break;
                    }
                }
            }
            Debug.Assert(foundCandidate);
        }
    }

    public static bool IsOnCurve(Vector2I coord, Vector2I pivot, float radius, float angle, bool clockwise=true)
    {
        var diff = coord - pivot;
        var testRad = Mathf.Atan2(-diff.Y, diff.X);
        var testAngle = NormalizeAngle((float)(testRad * (180.0 / Mathf.Pi)));
        if (!clockwise) testAngle = 360 - testAngle;
        if (testAngle >= angle) return false;
        var r1 = diff + new Vector2(0.5f, 0.5f);
        var r2 = diff + new Vector2(0.5f, -0.5f);
        var r3 = diff + new Vector2(-0.5f, -0.5f);
        var r4 = diff + new Vector2(-0.5f, 0.5f);
        var s1 = Mathf.Sign(r1.Length() - radius);
        var s2 = Mathf.Sign(r2.Length() - radius);
        var s3 = Mathf.Sign(r3.Length() - radius);
        var s4 = Mathf.Sign(r4.Length() - radius);
        return !(s1 * s2 >= 0 && s2 * s3 >= 0 && s3 * s4 >= 0);
    }
    
    public static Vector2 RotateCoord(Vector2 coord, Vector2 pivot, float angle)
    {
        var diff = coord - pivot;
        var rad = angle * Mathf.Pi / 180.0;
        var sin = Mathf.Sin(rad);
        var cos = Mathf.Cos(rad);
        var rotatedDiff = new Vector2((float)(diff.X * cos - diff.Y * sin), (float)(diff.X * sin + diff.Y * cos));
        var resFloat = coord + new Vector2(0.5f, 0.5f) + rotatedDiff;
        var resCoord = new Vector2I(Mathf.FloorToInt(resFloat.X), Mathf.FloorToInt(resFloat.Y));
        return resCoord;
    }

    public static IEnumerable<Vector2I> GetNeighbours4(Vector2I coord, bool takeDiagonal = false)
    {
        if (!takeDiagonal)
        {
            yield return new Vector2I(coord.X, coord.Y - 1);
            yield return new Vector2I(coord.X + 1, coord.Y);
            yield return new Vector2I(coord.X, coord.Y - 1);
            yield return new Vector2I(coord.X - 1, coord.Y);
        }
        else
        {
            yield return new Vector2I(coord.X + 1, coord.Y - 1);
            yield return new Vector2I(coord.X + 1, coord.Y + 1);
            yield return new Vector2I(coord.X - 1, coord.Y - 1);
            yield return new Vector2I(coord.X - 1, coord.Y + 1);
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

    public static int DirToAngle(Enums.Direction8 dir)
    {
        return dir switch
        {
            Enums.Direction8.North => 0,
            Enums.Direction8.NorthEast => 45,
            Enums.Direction8.East => 90,
            Enums.Direction8.SouthEast => 135,
            Enums.Direction8.South => 180,
            Enums.Direction8.SouthWest => 225,
            Enums.Direction8.West => 270,
            Enums.Direction8.NorthWest => 315,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
    }
    
    public static Vector2I DirToDxy(Enums.Direction9 direction9)
    {
        int x = 0, y = 0;
        switch (direction9 & (Enums.Direction9.Down | Enums.Direction9.Up))
        {
            case 0:
                break;
            case Enums.Direction9.Down:
                y = 1;
                break;
            case Enums.Direction9.Up:
                y = -1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction9), direction9, "Bitwise operation on direction9 is problematic");
        }
        switch (direction9 & (Enums.Direction9.Left | Enums.Direction9.Right))
        {
            case 0:
                break;
            case Enums.Direction9.Left:
                x = -1;
                break;
            case Enums.Direction9.Right:
                x = 1;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction9), direction9, "Bitwise operation on direction9 is problematic");
        }
        return new Vector2I(x, y);
    }

    public static Enums.Direction9 DxyToDir(Vector2I dxy)
    {
        int x = dxy.X, y = dxy.Y;
        var dir = Enums.Direction9.Neutral;
        if (x < 0) dir |= Enums.Direction9.Left;
        else if (x > 0) dir |= Enums.Direction9.Right;
        if (y < 0) dir |= Enums.Direction9.Up;
        else if (y > 0) dir |= Enums.Direction9.Down;
        return dir;
    }

    public static Enums.Direction8 AngleToDir(float angle)
    {
        angle = NormalizeAngle(angle);
        return angle switch
        {
            >= 337.5f or < 22.5f => Enums.Direction8.East,
            >= 22.5f and < 67.5f => Enums.Direction8.NorthEast,
            >= 67.5f and < 112.5f => Enums.Direction8.North,
            >= 112.5f and < 157.5f => Enums.Direction8.NorthWest,
            >= 157.5f and < 202.5f => Enums.Direction8.West,
            >= 202.5f and < 247.5f => Enums.Direction8.SouthWest,
            >= 247.5f and < 292.5f => Enums.Direction8.South,
            >= 292.5f and < 337.5f => Enums.Direction8.SouthEast,
            _ => throw new ArgumentOutOfRangeException(nameof(angle), angle, null)
        };
    }

    public static float NormalizeAngle(float angle)
    {
        angle %= 360;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
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

    public static bool GenericEquals<T>(T a, T b) where T: IEquatable<T>
    {
        if (ReferenceEquals(a, b))
        {
            // Both are null or reference the same object
            return true;
        }

        if (a is null || b is null)
        {
            // One of the values is null, but not both
            return false;
        }

        // Use Equals method for non-null values
        return a.Equals(b);
    }

    public static IEnumerable<Vector2I> IterateBoundingBox(Vector4I boundingBox)
    {
        for (int y = boundingBox.Y; y < boundingBox.Y + boundingBox.W; y++)
        {
            for (int x = boundingBox.X; x < boundingBox.X + boundingBox.Z; x++)
            {
                yield return new Vector2I(x, y);
            }
        }
    }

    public static Enums.Direction9 DirBetweenCoords(Vector2I a, Vector2I b)
    {
        var diff = b - a;
        if (diff == Vector2I.Zero)
        {
            return Enums.Direction9.Neutral;
        }
        var rad = Mathf.Atan2(-diff.Y, diff.X);
        var angle = rad * (180.0 / Mathf.Pi);
        return (Enums.Direction9)AngleToDir((float)angle);
    }

    public static bool IsAdjacent(Vector2I a, Vector2I b)
    {
        return Math.Abs(a.X - b.X) <= 1 && Math.Abs(a.Y - b.Y) <= 1;
    }
}