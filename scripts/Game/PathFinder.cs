using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game;

public class PathFinder
{
    private static Dictionary<Enums.FactionId, PathFinder> _pathFinders = new();
    
    private AStar2D _aStar;
    private Map _mapRef;
    private DuckObject _referenceCreature;
    
    protected PathFinder(DuckObject creature, Map map=null)
    {
        _mapRef = map ?? OnMap.GetMap(creature);
        _aStar = new AStar2D();
        _aStar.ReserveSpace(_mapRef.Size);
        _referenceCreature = creature;
    }

    public static PathFinder GetPathFinderFor(DuckObject creature, Map map=null)
    {
        map ??= OnMap.GetMap(creature);
        var factionId = Faction.GetFactionId(creature);
        if (!_pathFinders.TryGetValue(factionId, out var pathFinder))
        {
            pathFinder = new PathFinder(creature, map);
            pathFinder.Build();
            _pathFinders.Add(factionId, pathFinder);
        }
        return pathFinder;
    }


    public void Build()
    {
        _aStar.Clear();
        var coords = _mapRef.Coords.ToList();
        foreach (var coord in coords)
        {
            var index = _mapRef.CoordToIndex(coord);
            if (!_aStar.HasPoint(index))
            {
                _aStar.AddPoint(index, coord);
            }
        }
        foreach (var coord in coords)
        {
            var index = _mapRef.CoordToIndex(coord);
            var neighbours = Utils.GetNeighbours8(coord);
            foreach (var neighbour in neighbours)
            {
                if (!_mapRef.IsInBounds(neighbour))
                {
                    continue;
                }
                var neighborIndex = _mapRef.CoordToIndex(neighbour);
                // NOTE: we don't check for creatures here, creature check is done by setting point disabled
                if (_mapRef.IsWalkable(_referenceCreature, new[] { coord, neighbour }, ignoreCreatures:true))
                {
                    _aStar.ConnectPoints(index, neighborIndex, false);
                }
                if (_mapRef.IsWalkable(_referenceCreature, new[] { neighbour, coord }, ignoreCreatures:true))
                {
                    _aStar.ConnectPoints(neighborIndex, index, false);
                }
            }
        }
        foreach (var creature in _mapRef.Creatures)
        {
            if (Faction.LetPass(_referenceCreature, creature))
            {
                var coord = OnMap.GetCoord(creature);
                var index = _mapRef.CoordToIndex(coord);
                _aStar.SetPointDisabled(index);
            }
        }

        _mapRef.OnCreatureMoved += OnCreatureMoved;
        // TODO: register on creature move callback to adjust the pathfinder
    }

    public IEnumerable<Vector2I> FindPath(Vector2I from, Vector2I to)
    {
        var fromIndex = _mapRef.CoordToIndex(from);
        var toIndex = _mapRef.CoordToIndex(to);
        _aStar.SetPointDisabled(fromIndex, false);
        _aStar.SetPointDisabled(toIndex, false);
        var path = _aStar.GetIdPath(_mapRef.CoordToIndex(from), toIndex);
        _aStar.SetPointDisabled(fromIndex);
        _aStar.SetPointDisabled(toIndex);
        foreach (var index in path)
        {
            var coord = _mapRef.IndexToCoord((int)index);
            yield return coord;
        }
    }
    
    private void OnCreatureMoved(DuckObject creature, Vector2I fromCoord, Vector2I toCoord)
    {
        var fromIndex = _mapRef.CoordToIndex(fromCoord);
        var toIndex = _mapRef.CoordToIndex(toCoord);
        if (_aStar.HasPoint(fromIndex) && !_mapRef.HasObjectAt(fromCoord, Enums.DuckObjectTag.Creature) && _aStar.IsPointDisabled(fromIndex))
        {
            _aStar.SetPointDisabled(fromIndex, false);
        }
        if (_aStar.HasPoint(toIndex))
        {
            _aStar.SetPointDisabled(toIndex);
        }
    }
}