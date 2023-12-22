using System;
using System.Collections.Generic;
using System.Linq;
using DSS.Common;
using DSS.Game.Actions;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.TypedObjects;
using Godot;

namespace DSS.Game;

public class Map
{
    public Guid Id = Guid.NewGuid();
    public Vector2I Dimension;
    public bool InvertY;

    public byte[] GroundTiles;
    public byte[] WallTiles;
    public Action<Vector2I> OnTileChanged;
    
    public Dictionary<string, Dictionary<Guid, DuckObject>> TaggedObjects { get; } = new();
    
    public List<Room> Rooms { get; } = new();
    
    public int Size => Dimension.X * Dimension.Y;
    
    public Map(Vector2I dimension, bool invertY=true)
    {
        Dimension = dimension;
        InvertY = invertY;
        GroundTiles = new byte[Size];
        WallTiles = new byte[Size];
    }
    
    public Vector2I DirToDxy(Enums.Direction9 direction9)
    {
        int x = 0, y = 0;
        switch (direction9 & (Enums.Direction9.Down | Enums.Direction9.Up))
        {
            case 0:
                break;
            case Enums.Direction9.Down:
                y = -1;
                break;
            case Enums.Direction9.Up:
                y = 1;
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
        y = InvertY ? -y : y;
        return new Vector2I(x, y);
    }

    public Enums.Direction9 DxyToDir(Vector2I dxy)
    {
        int x = dxy.X, y;
        y = InvertY ? -dxy.Y : dxy.Y;
        var dir = Enums.Direction9.Neutral;
        if (x < 0) dir |= Enums.Direction9.Left;
        else if (x > 0) dir |= Enums.Direction9.Right;
        if (y < 0) dir |= Enums.Direction9.Down;
        else if (y > 0) dir |= Enums.Direction9.Up;
        return dir;
    }
    
    public Vector2I IndexToCoord(int index)
    {
        return new Vector2I(index % Dimension.X, index / Dimension.X); 
    }
    
    public int CoordToIndex(Vector2I coord)
    {
        return coord.X + coord.Y * Dimension.X;
    }
    
    public IEnumerable<DuckObject> GetObjectsAt(Vector2I coord, string tag=null)
    {
        foreach (var (objTag, objects) in TaggedObjects)
        {
            if (tag == null || tag == objTag)
            {
                foreach (var @object in objects.Values)
                {
                    if (coord.Equals(OnMapDuckObject.Coord(@object)))
                    {
                    
                        yield return @object;
                    }
                }
            }
        }
    }

    public BaseAction TryMoveObject(DuckObject obj, Enums.Direction9 dir)
    {
        if (dir == Enums.Direction9.Neutral)
        {
            return new StallAction(this, obj);
        }
        else
        {
            return new MoveAction(this, obj, dir);
        }
    }

    public bool MoveObject(DuckObject obj, Vector2I toCoord)
    {
        var fromCoord = OnMapDuckObject.Coord(obj);
        var path = Utils.GetLine(fromCoord, toCoord).ToArray();
        if (IsWalkable(obj, path))
        {
            OnMapDuckObject.Move(obj, toCoord);
            return true;
        }

        return false;
    }
    
    public bool IsWalkable(DuckObject walker, Vector2I[] path)
    {
        // TODO: Handle path with length above 2
        var toCoord = path[-1];
        if (toCoord.X < 0 || toCoord.X >= Dimension.X || toCoord.Y < 0 || toCoord.Y >= Dimension.Y)
        {
            return false;
        }
        var coord = OnMapDuckObject.Coord(walker);
        var index = CoordToIndex(coord);
        var res = true;
        res &= WallTiles[index] == 0;
        // TODO: check for objects at toCoord
        // TODO: check for directional passibility
        return res;
    }
}