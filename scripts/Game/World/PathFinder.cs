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
    private Game _gameRef;
    private Map _mapRef;
    private Entity _referenceCreature;
    
    public PathFinder(Game game, Enums.PathFindingFlag setting, Map map)
    {
        _gameRef = game;
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
            UpdatePointAtCoord(coord);
        }
        _mapRef.OnTileChanged += UpdatePointAtCoord;
    }

    public IEnumerable<Vector2I> Solve(Vector2I from, Vector2I to)
    {
        var fromIndex = _mapRef.CoordToIndex(from);
        var toIndex = _mapRef.CoordToIndex(to);
        _aStar.SetPointDisabled(fromIndex, false);
        _aStar.SetPointDisabled(toIndex, false);
        var path = _aStar.GetIdPath(fromIndex, toIndex);
        _aStar.SetPointDisabled(fromIndex);
        _aStar.SetPointDisabled(toIndex);
        foreach (var index in path)
        {
            var coord = _mapRef.IndexToCoord((int)index);
            yield return coord;
        }
    }

    private Entity CreateDummyCreature(Enums.PathFindingFlag setting)
    {
        var res = new Entity();
        var faction = (setting & Enums.PathFindingFlag.IsPlayer) != 0 ? Enums.FactionId.Player : Enums.FactionId.Monster;
        Faction.Setup(_gameRef, res, faction);
        // TODO: setup dummy creature with setting
        return res;
    }

    private void UpdatePointAtCoord(Vector2I coord)
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
            var isPassable1 = _mapRef.IsEdgeOpen(_referenceCreature, coord, neighbour, false);
            if (_aStar.ArePointsConnected(index, neighborIndex, false) ^ isPassable1)
            {
                if (isPassable1)
                {
                    _aStar.ConnectPoints(index, neighborIndex, false);
                }
                else
                {
                    _aStar.DisconnectPoints(index, neighborIndex, false);
                }
            }
            var isPassable2 = _mapRef.IsEdgeOpen(_referenceCreature, neighbour, coord, false);
            if (_aStar.ArePointsConnected(neighborIndex, index, false) ^ isPassable2)
            {
                if (isPassable2)
                {
                    _aStar.ConnectPoints(neighborIndex, index, false);
                }
                else
                {
                    _aStar.DisconnectPoints(neighborIndex, index, false);
                }
            }
        }

        foreach (var entity in _mapRef.FilterEntities(e => e.HasComp<Collider>()))
        {
            if (entity.GetComp<UniversalFlags>(ensure:false) is {} flagsComp && !flagsComp.HasFlags(Enums.EntityFlag.Dynamic))
            {
                continue;
            }
            _aStar.SetPointDisabled(index, true);
            break;
        }
        
    }
}