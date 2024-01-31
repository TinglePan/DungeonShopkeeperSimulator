using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using DSS.Common;
using DSS.Game.Actions;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;
using GoRogue.FOV;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace DSS.Game;

public class Map
{
    protected Game GameRef;
    public Guid Id = Guid.NewGuid();
    public Vector2I Dimension;

    public byte[] TerrainTiles;
    
    public Dictionary<Vector2I, HashSet<Guid>> SparseEntities = new Dictionary<Vector2I, HashSet<Guid>>();
    public WatchedBitArray VisibleTiles;
    public WatchedBitArray ExploredTiles;
    
    public Action<Vector2I> OnTileChanged;
    public Action<Vector2I> OnTileChangeWitnessed;
    
    public List<Room> Rooms = new ();
    
    // Derived states
    public WatchedDictionary<Guid, Entity> Entities = new ();
    public LambdaGridView<byte> WallTiles;
    public LambdaGridView<bool> TransparentTiles;
    
    // protected RecursiveShadowcastingBooleanBasedFOV Fov;
    protected Dictionary<Enums.PathFindingFlag, PathFinder> PathFinders = new ();
    protected Dictionary<Enums.ViewFlag, RecursiveShadowcastingBooleanBasedFOV> Fovs = new ();
    
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

    public IEnumerable<Entity> Creatures => FilterEntities(
        obj => obj.GetComp<Tagged>(ensure: false) is {Tag: Constants.EntityTagCreature });

    public IEnumerable<Entity> PlayerFactionCreatures => FilterEntities( 
        obj => 
            obj.GetComp<Tagged>(ensure: false) is {Tag: Constants.EntityTagCreature } &&
            obj.GetComp<Faction>().FactionId == Enums.FactionId.Player);

    public Entity PlayerControllingCreature => PlayerFactionCreatures.FirstOrDefault(
        creature => creature.GetComp<PlayerControl>().IsCurrentlyControlledBy(Enums.InputSource.LocalP1));

    public IEnumerable<Entity> FilterEntities(Func<Entity, bool> filter)
    {
        foreach (var @object in Entities.Values)
        {
            if (filter(@object))
            {
                yield return @object;
            }
        }
    }

    public Map(Game game, Vector2I dimension)
    {
        GameRef = game;
        Dimension = dimension;
        WallTiles = new LambdaGridView<byte>(dimension.X, dimension.Y,
            point =>
            {
                var coord = new Vector2I(point.X, point.Y);
                var tileEntity = GetObjectsAt(coord, e => e.GetComp<Tagged>(ensure: false) is {Tag: Constants.EntityTagWall })?.FirstOrDefault();
                return (byte)(tileEntity?.GetComp<Tile>().TileId ?? 0);
            });
        VisibleTiles = new WatchedBitArray(Size);
        ExploredTiles = new WatchedBitArray(Size);
        TransparentTiles = new LambdaGridView<bool>(dimension.X, dimension.Y, IsTransparentAt);
        
        VisibleTiles.OnElementChanged += (index, oldValue, newValue) =>
        {
            var coord = IndexToCoord(index);
            foreach (var obj in GetObjectsAt(coord))
            {
                if (obj.GetComp<Renderable>(ensure: false) is { HideUnseen: true } renderableComp)
                {
                    // GD.Print($"Setting visible to {newValue}");
                    obj.GetComp<Renderable>().IsVisible.Value = newValue;
                }
            }
            OnTileChanged?.Invoke(IndexToCoord(index));
            if (VisibleTiles[index])
            {
                OnTileChangeWitnessed?.Invoke(coord);
            }
        };
    }
    
    public Vector2I IndexToCoord(int index)
    {
        return new Vector2I(index % Dimension.X, index / Dimension.X); 
    }
    
    public int CoordToIndex(Vector2I coord)
    {
        return coord.X + coord.Y * Dimension.X;
    }
    
    public IEnumerable<Entity> GetObjectsAt(Vector2I coord, Func<Entity, bool> filter=null)
    {
        if (!SparseEntities.TryGetValue(coord, out var objectIds)) yield break;
        foreach (var objectId in objectIds)
        {
            var obj = Entities[objectId];
            if (filter == null || filter(obj))
            {
                yield return obj;
            }
        }
    }
    
    public bool HasObjectAt(Vector2I coord, Func<Entity, bool> filter=null)
    {
        if (!SparseEntities.TryGetValue(coord, out var objectIds)) return false;
        foreach (var objectId in objectIds)
        {
            var obj = Entities[objectId];
            if (filter == null || filter(obj))
            {
                return true;
            }
        }
        return false;
    }
    
    public void SpawnObject(Entity obj, Vector2I coord, string tag=null)
    {
        OnMap.Setup(GameRef, obj, this, coord);
        if (tag != null)
        {
            Tagged.Setup(GameRef, obj, tag);
        }
        Entities.Add(obj.Id, obj);
        SparseEntities.TryAdd(coord, new HashSet<Guid>());
        SparseEntities[coord].Add(obj.Id);
        obj.GetComp<OnMap>().Coord.OnChanged += (fromCoord, toCoord) =>
        {
            SparseEntities[fromCoord].Remove(obj.Id);
            if (!SparseEntities[fromCoord].Any()) SparseEntities.Remove(fromCoord);
            SparseEntities.TryAdd(toCoord, new HashSet<Guid>());
            SparseEntities[toCoord].Add(obj.Id);
            if (obj.GetComp<Viewshed>(ensure: false) is {} viewshedComp)
            {
                if (viewshedComp.ProvidePlayerVision())
                {
                    viewshedComp.OnRegionChanged += UpdateVisibleTiles;
                }
                viewshedComp.UpdateRegion();
            }
        };
        if (obj.GetComp<Viewshed>(ensure: false) is {} viewshedComp2)
        {
            if (viewshedComp2.ProvidePlayerVision())
            {
                viewshedComp2.OnRegionChanged += UpdateVisibleTiles;
            }
            viewshedComp2.UpdateRegion();
        }
    }
    
    public bool IsPathOpen(Entity entity, Vector2I[] path, bool checkDynamicEntities=false)
    {
        for (int i = 0; i < path.Length - 1; i++)
        {
            var fromCoord = path[i];
            var toCoord = path[i + 1];
            if (!IsEdgeOpen(entity, fromCoord, toCoord, checkDynamicEntities)) return false;
        }
        return true;
    }

    public bool IsEdgeOpen(Entity entity, Vector2I fromCoord, Vector2I toCoord, bool checkDynamicEntities=true)
    {
        if (!IsInBounds(toCoord))
        {
            return false;
        }

        if (!(entity.GetComp<Collider>(ensure: false) is { } entityColliderComp)) return true;

        foreach (var maybeObstacle in GetObjectsAt(toCoord))
        {
            if (!(maybeObstacle.GetComp<Collider>(ensure: false) is { } targetColliderComp))
            {
                continue;
            }
            if (!checkDynamicEntities && (maybeObstacle.GetComp<UniversalFlags>(ensure:false)?.HasFlags(Enums.EntityFlag.Dynamic) ?? false))
            {
                continue;
            }
            var obstacleColliderComp = maybeObstacle.GetComp<Collider>();
            if (!entityColliderComp.CanPass(obstacleColliderComp))
            {
                continue;
            }
            // Blocked by obstacle
            return false;
        }
        return true;
    }
    
    public bool IsInBounds(Vector2I coord)
    {
        return coord.X >= 0 && coord.X < Dimension.X && coord.Y >= 0 && coord.Y < Dimension.Y;
    }

    public IEnumerable<Vector2I> Navigate(Vector2I fromCoord, Vector2I toCoord, bool isPlayer=true)
    {
        // TODO: add other pathfinding settings
        Enums.PathFindingFlag pathFindSetting = 0;
        pathFindSetting |= isPlayer ? Enums.PathFindingFlag.IsPlayer : 0;
        
        if (!PathFinders.TryGetValue(pathFindSetting, out var pathFinder))
        {
            pathFinder = new PathFinder(GameRef, pathFindSetting, this);
            PathFinders[pathFindSetting] = pathFinder;
        }
        return pathFinder.Solve(fromCoord, toCoord);
    }

    // protected void TryMoveObjectPart(Entity entity, Vector2I fromCoord, Enums.Direction8 dir, out bool isMove, out bool isBump)
    // {
    //     isMove = false;
    //     isBump = false;
    //     var toCoord = fromCoord + Utils.DirToDxy((Enums.Direction9)dir);
    //     var toIndex = CoordToIndex(toCoord);
    //     if (!IsInBounds(toCoord))
    //     {
    //         return;
    //     }
    //
    //     if (TerrainTilesView[toIndex] != 0)
    //     {
    //         var terrainTileDef = Game.Instance.DefStore.GetTileDef(TerrainTilesView[toIndex]);
    //         // TODO: handle terrain passage, water, lava, cliff, etc.
    //     }
    //
    //     if (WallTilesView[toIndex] != 0)
    //     {
    //         // TODO: handle wall phasing abilities, handle wall destruction abilities
    //         return;
    //     }
    //
    //     var entityMultiTilePartComp = entity.GetComp<MultiTilePart>(ensure: false);
    //     foreach (var obstacle in GetObjectsAt(toCoord, obj => obj.GetComp<UniversalFlags>(ensure:false)?.HasFlags(Enums.DuckObjectFlag.Obstacle) ?? false))
    //     {
    //         switch (obstacle.GetComp<Tagged>(ensure:false)?.Tag)
    //         {
    //             case Constants.DuckObjectTagCreature:
    //                 break;
    //             case Constants.DuckObjectTagFurniture:
    //                 break;
    //         }
    //         // Does not collide with 
    //         var obstacleMultiTilePartComp = entity.GetComp<MultiTilePart>(ensure:false);
    //         if (entityMultiTilePartComp != null && obstacleMultiTilePartComp != null &&
    //             entityMultiTilePartComp.ParentRef.Equals(obstacleMultiTilePartComp.ParentRef))
    //         {
    //             continue;
    //         }    
    //         var entityFactionComp = entity.GetComp<Faction>();
    //         var obstacleFactionComp = obstacle.GetComp<Faction>(ensure: false);
    //         if (entityFactionComp.IsHostile(obstacleFactionComp))
    //         {
    //             isBump = true;
    //             return;
    //         }
    //         // TODO: other bump action circumstances
    //         // NOTE: only player is allowed to pass through friendly creatures
    //         if (!entity.Equals(PlayerControllingCreature) || obstacleFactionComp == null)
    //         {
    //             return;
    //         }
    //     }
    // }

    // public IEnumerable<BaseAction> TryMoveObject(Entity entity, Enums.Direction8 dir)
    // {
    //     bool isMove, isBump;
    //     if (entity.GetComp<MultiTile>(ensure: false) is { } multiTileComp)
    //     {
    //         var res = new List<BaseAction>();
    //         foreach (var (fromCoord, partObj) in multiTileComp.Tiles)
    //         {
    //             TryMoveObjectPart(partObj, fromCoord, dir, out isMove, out isBump);
    //             if (isBump)
    //             {
    //                 res.Add(new BumpAction(GameRef, partObj, dir));
    //             }
    //             else if (isMove)
    //             {
    //                 res.Add(new MoveAction(GameRef, partObj, dir));
    //             }
    //             else
    //             {
    //                 yield break;
    //             }
    //         }
    //         foreach (var action in res)
    //         {
    //             yield return action;
    //         }
    //     }
    //     else
    //     {
    //         var onMapComp = entity.GetComp<OnMap>();
    //         var fromCoord = onMapComp.Coord.Value;
    //         TryMoveObjectPart(entity, fromCoord, dir, out isMove, out isBump);
    //         if (isBump)
    //         {
    //             yield return new BumpAction(GameRef, entity, dir);
    //         }
    //         else if (isMove)
    //         {
    //             yield return new MoveAction(GameRef, entity, dir);
    //         }
    //     }
    // }
    
    public void MoveObject(Entity entity, Enums.Direction8 dir)
    {
        var onMapComp = entity.GetComp<OnMap>();
        var fromCoord = onMapComp.Coord.Value;
        var toCoord = fromCoord + Utils.DirToDxy((Enums.Direction9)dir);
        
        // Move
        if (entity.GetComp<FaceDir>(ensure: false) is { } faceDirComp)
        {
            faceDirComp.Dir.Value = (int)dir;
        }
        onMapComp.Coord.Value = toCoord;
        OnTileChanged?.Invoke(fromCoord);
        if (VisibleTiles[CoordToIndex(fromCoord)])
        {
            OnTileChangeWitnessed?.Invoke(fromCoord);
        }
        OnTileChanged?.Invoke(toCoord);
        if (VisibleTiles[CoordToIndex(toCoord)])
        {
            OnTileChangeWitnessed?.Invoke(toCoord);
        }
    }
    
    public IEnumerable<Vector2I> GetVisibleTiles(Vector2I atCoord, Enums.Direction8 faceDir, int angle, int range, bool isXRay=false)
    {
        Enums.ViewFlag viewSetting = 0;
        if (isXRay) viewSetting |= Enums.ViewFlag.XRay;
        if (!Fovs.TryGetValue(viewSetting, out var fov))
        {
            fov = ResolveFov(viewSetting);
            Fovs[viewSetting] = fov;
        }
        fov.Calculate(atCoord.X, atCoord.Y, radius:range, distanceCalc:Radius.Circle, angle:Utils.DirToAngle(faceDir), span: angle);
        var res = fov.BooleanResultView;
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

    protected bool IsTransparentAt(Point pos)
    {
        // TODO: directional transparency?
        var coord = new Vector2I(pos.X, pos.Y);
        foreach (var @object in GetObjectsAt(coord))
        {
            if (@object.GetComp<UniversalFlags>(ensure:false)?.HasFlags(Enums.EntityFlag.Occlusion) ?? false)
            {
                return false;
            }
        }
        return true;
    }
    
    protected void UpdateVisibleTiles(HashSet<Vector2I> from, HashSet<Vector2I> to)
    {
        var newlySeen = to != null ? (from != null ? to.Except(from) : to) : Enumerable.Empty<Vector2I>();
        var maybeUnseen = from != null ? (to != null ? from.Except(to) : from) : Enumerable.Empty<Vector2I>();
        foreach (var coord in newlySeen)
        {
            var index = CoordToIndex(coord);
            VisibleTiles[index] = true;
            ExploredTiles[index] = true;
            OnTileChangeWitnessed?.Invoke(coord);
        }

        Profile.StartWatch("UpdateVisibleTiles");
        foreach (var coord in maybeUnseen)
        {
            bool isUnseen = true;
            Profile.StartWatch("UpdateVisibleTiles 1");
            foreach (var obj in Entities.Values)
            {
                Profile.StartWatch("UpdateVisibleTiles 2");
                if (!(obj.GetComp<Viewshed>(ensure: false) is {} viewshedComp)) continue;
                Profile.EndWatch("UpdateVisibleTiles 2", true, 100);
                Profile.StartWatch("UpdateVisibleTiles 3");
                if (!viewshedComp.ProvidePlayerVision()) continue;
                Profile.EndWatch("UpdateVisibleTiles 3", true, 100);
                Profile.StartWatch("UpdateVisibleTiles 4");
                if (viewshedComp.Region.Contains(coord))
                {
                    isUnseen = false;
                    Profile.EndWatch("UpdateVisibleTiles 4", true, 100);
                    break;
                }
                Profile.EndWatch("UpdateVisibleTiles 4", true, 100);
            }
            Profile.EndWatch("UpdateVisibleTiles 1", true, 100);
            if (isUnseen)
            {
                var index = CoordToIndex(coord);
                VisibleTiles[index] = false;
                OnTileChangeWitnessed?.Invoke(coord);
            }
        }
        Profile.EndWatch("UpdateVisibleTiles", true, 10);
    }
    
    protected Enums.Direction8 FaceDirAfterPath(Vector2I[] path)
    {
        var lastTwo = path[^2..];
        var dxy = lastTwo[1] - lastTwo[0];
        return (Enums.Direction8)Utils.DxyToDir(dxy);
    }
    
    protected RecursiveShadowcastingBooleanBasedFOV ResolveFov(Enums.ViewFlag setting)
    {
        // TODO: handle other view flags
        RecursiveShadowcastingBooleanBasedFOV res;
        if ((setting & Enums.ViewFlag.XRay) != 0)
        {
            res = new RecursiveShadowcastingBooleanBasedFOV(new LambdaGridView<bool>(
                Dimension.X, Dimension.Y, _ => true));
        }
        else
        {
            res = new RecursiveShadowcastingBooleanBasedFOV(TransparentTiles);
        }
        return res;
    }
}