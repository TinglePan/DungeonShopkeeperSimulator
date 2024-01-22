using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DSS.Common;
using DSS.Game.Actions;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using DSS.Game.DuckTyping.Systems;
using Godot;
using GoRogue.FOV;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace DSS.Game;

public class Map
{
    public Guid Id = Guid.NewGuid();
    public Vector2I Dimension;
    public bool InvertY;
    
    public byte[] GroundTiles;
    public WatchedArray<byte> WallTiles;
    public WatchedArray<byte> PlayerTileVisibilities;
    
    public Dictionary<Enums.DuckObjectTag, Dictionary<Guid, DuckObject>> TaggedObjects = new ();
    public List<Room> Rooms = new ();
    
    public Action<Vector2I> OnTileChanged;
    public Action<Vector2I> OnTileChangeWitnessed;
    
    // Derived states
    protected BitArray Transparency;
    protected RecursiveShadowcastingBooleanBasedFOV Fov;
    protected Dictionary<Enums.PathFindingFlag, PathFinder> PathFinders = new ();
    
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

    public IEnumerable<DuckObject> PlayerSideCreatures
    {
        get
        {
            if (TaggedObjects.TryGetValue(Enums.DuckObjectTag.Creature, out var objects))
            {
                foreach (var creature in objects.Values)
                {
                    if (Faction.GetFactionId(creature) == Enums.FactionId.Player)
                    {
                        yield return creature;
                    }
                }
            }
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

    public IEnumerable<DuckObject> FilterObjects(Func<DuckObject, bool> filter)
    {
        foreach (var (objTag, objects) in TaggedObjects)
        {
            foreach (var @object in objects.Values)
            {
                if (filter(@object))
                {
                    yield return @object;
                }
            }
        }
    }

    public Map(Vector2I dimension, bool invertY = true)
    {
        Dimension = dimension;
        InvertY = invertY;
        GroundTiles = new byte[Size];
        WallTiles = new WatchedArray<byte>(Size);
        Transparency = new BitArray(Size);
        Transparency.SetAll(true);
        PlayerTileVisibilities = new WatchedArray<byte>(Size);
        PlayerTileVisibilities.Fill((byte)Enums.TileVisibility.Hidden);
        PlayerTileVisibilities.OnElementChanged += (index, oldValue, newValue) =>
        {
            foreach (var obj in GetObjectsAt(IndexToCoord(index)))
            {
                if (obj.GetComp<Renderable>(ensure: false) is { HideWhenNotVisible: true } renderableComp)
                {
                    renderableComp.IsVisible.Value = newValue == (byte)Enums.TileVisibility.Visible;
                }
            }
            OnTileChanged?.Invoke(IndexToCoord(index));
            if (oldValue == (byte)Enums.TileVisibility.Visible || newValue == (byte)Enums.TileVisibility.Visible)
            {
                OnTileChangeWitnessed?.Invoke(IndexToCoord(index));
            }
        };
        WallTiles.OnElementChanged += (index, oldValue, newValue) =>
        {
            OnTileChanged?.Invoke(IndexToCoord(index));
            if (PlayerTileVisibilities[index] == (byte)Enums.TileVisibility.Visible)
            {
                OnTileChangeWitnessed?.Invoke(IndexToCoord(index));
            }
            UpdateTransparencyAt(IndexToCoord(index));
        };
        Fov = new RecursiveShadowcastingBooleanBasedFOV(new BitArrayView(Transparency, Dimension.X));
        // TODO: update transparency according to tile change instead of wall tile change
        // OnTileChanged += UpdateTransparencyAt;
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
        OnMap.Setup(obj, this, coord);
        if (!TaggedObjects.TryGetValue(tag, out var objects))
        {
            objects = new Dictionary<Guid, DuckObject>();
            TaggedObjects[tag] = objects;
        }
        objects[obj.Id] = obj;
        if (obj.GetComp<PlayerViewshed>(ensure: false) is { } playerViewshed)
        {
            Viewshed.WatchVisibilityChange(obj, UpdatePlayerVisibilities);
        }
        if (obj.GetComp<Viewshed>(ensure: false) is not { } viewshed) return;
        OnMap.WatchCoordChange(obj, (_, _) => VisibilitySystem.UpdateVisibilityOf(obj));
        VisibilitySystem.UpdateVisibilityOf(obj);
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
        if (IsPassable(obj, path))
        {
            var dir = FaceDirAfterPath(path);
            if (obj.GetComp<FaceDir>(ensure: false) is { } faceDirComp)
            {
                FaceDir.SetFaceDir(obj, dir);
            }
            OnMap.Move(obj, toCoord);
            OnTileChanged?.Invoke(fromCoord);
            if (PlayerTileVisibilities[CoordToIndex(fromCoord)] == (byte)Enums.TileVisibility.Visible)
            {
                OnTileChangeWitnessed?.Invoke(fromCoord);
            }
            OnTileChanged?.Invoke(toCoord);
            if (PlayerTileVisibilities[CoordToIndex(toCoord)] == (byte)Enums.TileVisibility.Visible)
            {
                OnTileChangeWitnessed?.Invoke(toCoord);
            }
            return true;
        }

        return false;
    }
    
    public bool IsPassable(DuckObject walker, Vector2I[] path, bool ignoreCreatures=false)
    {
        // TODO: Handle path with length above 2
        for (int i = 0; i < path.Length - 1; i++)
        {
            var fromCoord = path[i];
            var toCoord = path[i + 1];
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
        }
        return true;
    }

    public bool IsInBounds(Vector2I coord)
    {
        return coord.X >= 0 && coord.X < Dimension.X && coord.Y >= 0 && coord.Y < Dimension.Y;
    }

    public IEnumerable<Vector2I> NavigateTo(DuckObject entity, Vector2I targetPos)
    {
        var pathFindSetting = GetPathFindSetting(entity);
        if (!PathFinders.TryGetValue(pathFindSetting, out var pathFinder))
        {
            pathFinder = new PathFinder(pathFindSetting, this);
            PathFinders[pathFindSetting] = pathFinder;
        }
        var fromPos = OnMap.GetCoord(entity);
        return pathFinder.Solve(fromPos, targetPos);
    }

    public IEnumerable<Vector2I> GetVisibleCoords(Vector2I atCoord, Enums.Direction8 faceDir, int angle, int range)
    {
        Fov.Calculate(atCoord.X, atCoord.Y, radius:range, distanceCalc:Radius.Circle, angle:Utils.DirToAngle(faceDir), span: angle);
        var res = Fov.BooleanResultView;
        for (var y = 0; y < Dimension.Y; y++)
        {
            for (var x = 0; x < Dimension.X; x++)
            {
                var index = CoordToIndex(new Vector2I(x, y));
                if (res[index])
                {
                    yield return new Vector2I(x, y);
                }
            }
        }
    }

    protected void UpdateTransparencyAt(Vector2I coord)
    {
        // TODO: handle building and creature transparency
        // TODO: directional transparency?
        // TODO: check wall def to see if it's transparent
        var index = CoordToIndex(coord);
        var wallTile = WallTiles[index];
        var isTransparent = wallTile == 0;
        Transparency[index] = isTransparent;
    }
    
    protected Enums.PathFindingFlag GetPathFindSetting(DuckObject entity)
    {
        Enums.PathFindingFlag flags = 0;
        bool isPlayer = Faction.GetFactionId(entity) == Enums.FactionId.Player;
        flags |= isPlayer ? Enums.PathFindingFlag.IsPlayer : 0;
        return flags;
    }
    
    protected void UpdatePlayerVisibilities()
    {
        for (int i = 0; i < PlayerTileVisibilities.Length; i++)
        {
            PlayerTileVisibilities[i] |= (byte)Enums.TileVisibility.Revealed;
        }
        foreach (var obj in FilterObjects((obj) => obj.GetComp<PlayerViewshed>(ensure:false) != null))
        {
            foreach (var visibleCoord in Viewshed.GetVisibleCoords(obj))
            {
                PlayerTileVisibilities[CoordToIndex(visibleCoord)] &= (byte)Enums.TileVisibility.Visible;
            }
        }
    }
    
    protected Enums.Direction8 FaceDirAfterPath(Vector2I[] path)
    {
        var lastTwo = path[^2..];
        var dxy = lastTwo[1] - lastTwo[0];
        return (Enums.Direction8)DxyToDir(dxy);
    }
}