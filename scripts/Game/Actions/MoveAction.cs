using System.Diagnostics;
using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.Actions;

public class MoveAction: ActionWithDirection
{
    protected Map MapRef;
    protected DuckObject EntityRef;
    
    public MoveAction(Map map, DuckObject entity, Enums.Direction9 dir): base(dir)
    {
        MapRef = map;
        EntityRef = entity;
    }
    
    public override bool Execute()
    {
        var onMapComp = EntityRef.GetComp<OnMap>();
        Debug.Assert(onMapComp != null);
        Debug.Assert(onMapComp.MapId == MapRef.Id);
        // TODO: dxy should depend on entity
        var dxy = MapRef.DirToDxy(Dir);
        var from = onMapComp.Coord;
        var to = from + dxy;
        return MapRef.MoveObject(EntityRef, to);
    }
}