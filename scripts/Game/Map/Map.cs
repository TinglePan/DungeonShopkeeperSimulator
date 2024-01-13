using System;
using System.Collections.Generic;
using System.Linq;
using DSS.Common;
using DSS.Game.Actions;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
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
    public Action<DuckObject, Vector2I, Vector2I> OnCreatureMoved;
    
    public Dictionary<Enums.DuckObjectTag, Dictionary<Guid, DuckObject>> TaggedObjects { get; } = new();
    
    public List<Room> Rooms { get; } = new();
    
    public int Size => Dimension.X * Dimension.Y;
    
    public IEnumerable<Vector2I> Coords {
        get
        {
            for (var y = 0; y < Dimension.Y; y++)
            {
                for (var x = 0; x < Dimension.X; x++)
                {
                    yield return new Vector2I(x, y);
                }
            }
        }
    }

    public IEnumerable<DuckObject> Creatures
    {
        get
        {
            if (TaggedObjects.TryGetValue(Enums.DuckObjectTag.Creature, out var objects))
            {
                return objects.Values;
            }
            return Enumerable.Empty<DuckObject>();
        }
    }

    public DuckObject Player
    {
        get
        {
            if (TaggedObjects.TryGetValue(Enums.DuckObjectTag.Creature, out var objects))
            {
                foreach (var creature in objects.Values)
                {
                    if (PlayerControl.IsCurrentlyControlledBy(creature, Enums.InputSource.LocalP1))
                    {
                        return creature;
                    }
                }
            }
            return null;
        }
        
    }
    
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
    
    public IEnumerable<DuckObject> GetObjectsAt(Vector2I coord, Enums.DuckObjectTag tag=default)
    {
        foreach (var (objTag, objects) in TaggedObjects)
        {
            if (tag == default || tag == objTag)
            {
                foreach (var @object in objects.Values)
                {
                    if (coord.Equals(OnMap.GetCoord(@object)))
                    {
                        yield return @object;
                    }
                }
            }
        }
    }
    
    public bool HasObjectAt(Vector2I coord, Enums.DuckObjectTag tag=default)
    {
        foreach (var (objTag, objects) in TaggedObjects)
        {
            if (tag == default || tag == objTag)
            {
                foreach (var @object in objects.Values)
                {
                    if (coord.Equals(OnMap.GetCoord(@object)))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    public void SpawnObject(DuckObject obj, Vector2I coord, Enums.DuckObjectTag tag=default)
    {
        var onMapComp = obj.GetComp<OnMap>();
        if (onMapComp == null)
        {
            onMapComp = new OnMap() { Coord = coord, MapId = Id };
            obj.AddComp(onMapComp);
        }
        else
        {
            onMapComp.Coord = coord;
        }
        if (!TaggedObjects.TryGetValue(tag, out var objects))
        {
            objects = new Dictionary<Guid, DuckObject>();
            TaggedObjects[tag] = objects;
        }
        objects[obj.Id] = obj;
    }

    public BaseAction TryMoveObject(DuckObject obj, Vector2I toCoord)
    {
        if (OnMap.GetCoord(obj) == toCoord)
        {
            return new StallAction(obj);
        }
        else
        {
            return new MoveAction(obj, toCoord);
        }
    }

    public bool MoveObject(DuckObject obj, Vector2I toCoord)
    {
        var fromCoord = OnMap.GetCoord(obj);
        var path = Utils.GetLine(fromCoord, toCoord).ToArray();
        if (IsWalkable(obj, path))
        {
            OnMap.Move(obj, toCoord);
            OnCreatureMoved?.Invoke(obj, fromCoord, toCoord);
            OnTileChanged?.Invoke(fromCoord);
            OnTileChanged?.Invoke(toCoord);
            return true;
        }

        return false;
    }
    
    public bool IsWalkable(DuckObject walker, Vector2I[] path, bool ignoreCreatures=false)
    {
        // TODO: Handle path with length above 2
        var toCoord = path.Last();
        if (!IsInBounds(toCoord))
        {
            return false;
        }
        var index = CoordToIndex(toCoord);
        if (WallTiles[index] != 0)
        {
            // TODO: Handle creature that can pass wall
            return false;
        }
        // TODO: check for directional pass
        if (!ignoreCreatures)
        {
            foreach (var creature in Creatures)
            {
                var creatureCoord = OnMap.GetCoord(creature);
                if (creatureCoord.Equals(toCoord))
                {
                    if (Faction.LetPass(walker, creature)) continue;
                    return false;
                }
            }
        }
        // TODO: check for buildings at toCoord
        return true;
    }

    public bool IsInBounds(Vector2I coord)
    {
        return coord.X >= 0 && coord.X < Dimension.X && coord.Y >= 0 && coord.Y < Dimension.Y;
    }

    public IEnumerable<Vector2I> GetVisibleCoords(Vector2I atCoord, Enums.Direction8 faceDir, int angle, int range)
    {
        yield break;
    }
}