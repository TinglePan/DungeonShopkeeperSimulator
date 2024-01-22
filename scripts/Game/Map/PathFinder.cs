using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game;

public class PathFinder
{
    private AStar2D _aStar;
    private Map _mapRef;
    private DuckObject _referenceCreature;
    
    public PathFinder(Enums.PathFindingFlag setting, Map map)
    {
        _mapRef = map;
        _aStar = new AStar2D();
        _aStar.ReserveSpace(_mapRef.Size);
        _referenceCreature = CreateDummyCreature(setting);
        Build();
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
                if (_mapRef.IsPassable(_referenceCreature, new[] { coord, neighbour }, ignoreCreatures:true))
                {
                    _aStar.ConnectPoints(index, neighborIndex, false);
                }
                if (_mapRef.IsPassable(_referenceCreature, new[] { neighbour, coord }, ignoreCreatures:true))
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

        _mapRef.OnTileChanged += UpdateTile;
        // TODO: register on creature move callback to adjust the pathfinder
    }

    public IEnumerable<Vector2I> Solve(Vector2I from, Vector2I to)
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
    
    private void UpdateTile(Vector2I coord)
    {
        var index = _mapRef.CoordToIndex(coord);
        Debug.Assert(_aStar.HasPoint(index));
        _aStar.SetPointDisabled(index, _mapRef.HasObjectAt(coord, Enums.DuckObjectTag.Creature));
    }

    private DuckObject CreateDummyCreature(Enums.PathFindingFlag setting)
    {
        var res = new DuckObject();
        var faction = (setting & Enums.PathFindingFlag.IsPlayer) != 0 ? Enums.FactionId.Player : Enums.FactionId.Monster;
        Faction.Setup(res, faction);
        // TODO: setup dummy creature with setting
        return res;
    }
}