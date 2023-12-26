using System;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class OnMap: BaseComp
{
    public Guid MapId;
    public Vector2I Coord;
    
    public Action<Vector2I, Vector2I> OnCoordChanged;
    
    public static bool CheckDuckType(DuckObject obj)
    {
        return obj.GetComp<OnMap>() != null;
    }

    public static void Setup(DuckObject obj, Map map, Vector2I coord)
    {
        var onMapComp = new OnMap
        {
            MapId = map.Id,
            Coord = coord,
        };
        obj.AddComp(onMapComp);
    }
    
    public static void RegisterCoordChangeCallback(DuckObject obj, Action<Vector2I, Vector2I> onCoordChanged)
    {
        var onMapComp = obj.GetComp<OnMap>();
        onMapComp.OnCoordChanged += onCoordChanged;
    }
    
    public static Vector2I GetCoord(DuckObject obj)
    {
        var onMapComp = obj.GetComp<OnMap>();
        return onMapComp.Coord;
    }

    public static void Move(DuckObject obj, Vector2I toCoord)
    {
        var onMapComp = obj.GetComp<OnMap>();
        if (onMapComp.OnCoordChanged != null)
        {
            onMapComp.OnCoordChanged.Invoke(onMapComp.Coord, toCoord);
        }
        onMapComp.Coord = toCoord;
    }

    public static Map GetMap(DuckObject obj)
    {
        var onMapComp = obj.GetComp<OnMap>();
        var mapId = onMapComp.MapId;
        return Game.Instance.GameState.Maps.TryGetValue(mapId, out var map) ? map : null;
    }
    
}