using DSS.Common;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.DuckTyping.TypedObjects;

public static class OnMapDuckObject
{
    public static bool CheckDuckType(DuckObject obj)
    {
        return obj.GetComp<OnMap>() != null;
    }

    public static void Setup(DuckObject obj, Map map, Vector2I coord)
    {
        var onMapComp = new OnMap
        {
            MapId = map.Id,
            Coord = coord
        };
        obj.AddComp(onMapComp);
    }
    
    public static Vector2I Coord(DuckObject obj)
    {
        var onMapComp = obj.GetComp<OnMap>();
        return onMapComp.Coord;
    }

    public static void Move(DuckObject obj, Vector2I toCoord)
    {
        var onMapComp = obj.GetComp<OnMap>();
        onMapComp.Coord = toCoord;
    }

    public static Map Map(DuckObject obj)
    {
        var onMapComp = obj.GetComp<OnMap>();
        var mapId = onMapComp.MapId;
        return Game.Instance.GameState.Maps.TryGetValue(mapId, out var map) ? map : null;
    }
}