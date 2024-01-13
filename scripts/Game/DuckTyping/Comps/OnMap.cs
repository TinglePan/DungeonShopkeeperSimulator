using System;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class OnMap: BaseComp
{
    public Guid MapId;
    public Vector2I Coord;
    
    public Action<Vector2I, Vector2I> OnCoordChanged;

    public static void Setup(DuckObject obj, Map map, Vector2I coord)
    {
        var onMapComp = obj.GetCompOrNew<OnMap>();
        onMapComp.MapId = map.Id;
        onMapComp.Coord = coord;
    }
    
    public static void WatchCoordChange(DuckObject obj, Action<Vector2I, Vector2I> onCoordChanged)
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
        var fromCoord = onMapComp.Coord;
        onMapComp.Coord = toCoord;
        if (onMapComp.OnCoordChanged != null)
        {
            onMapComp.OnCoordChanged.Invoke(fromCoord, toCoord);
        }
    }

    public static Map GetMap(DuckObject obj)
    {
        var onMapComp = obj.GetComp<OnMap>();
        var mapId = onMapComp.MapId;
        return Game.Instance.GameState.Maps.TryGetValue(mapId, out var map) ? map : null;
    }
    
    public static bool IsOnSameMap(DuckObject obj1, DuckObject obj2)
    {
        var onMapComp1 = obj1.GetComp<OnMap>();
        var onMapComp2 = obj2.GetComp<OnMap>();
        return onMapComp1.MapId == onMapComp2.MapId;
    }
    
}