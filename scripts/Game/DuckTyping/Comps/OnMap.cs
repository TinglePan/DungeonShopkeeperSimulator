using System;
using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class OnMap: BaseComp
{
    public Map Map;
    public Watched<Vector2I> Coord = new (default);

    public static void Setup(Game game, Entity obj, Map map, Vector2I coord)
    {
        var onMapComp = obj.GetCompOrNew<OnMap>();
        onMapComp.GameRef = game;
        onMapComp.EntityRef = obj;
        onMapComp.Map = map;
        onMapComp.Coord.Value = coord;
    }

    public void Move(Enums.Direction8 dir)
    {
        var toCoord = Coord.Value + Utils.DirToDxy((Enums.Direction9)dir);
        Move(toCoord);
    }

    public void Move(Vector2I toCoord)
    {
        if (!Utils.IsAdjacent(Coord.Value, toCoord))
        {
            GD.PrintErr("Trying to move to non-adjacent tile, should use Teleport");
        }
        else
        {
            Coord.Value = toCoord;
        }
    }
}