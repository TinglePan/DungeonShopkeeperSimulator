using System.Collections.Generic;
using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Faction: BaseComp
{
    public Enums.FactionId FactionId;

    public static void Setup(DuckObject obj, Enums.FactionId factionId)
    {
        var factionComp = obj.GetCompOrNew<Faction>();
        factionComp.FactionId = factionId;
    }

    public static bool IsHostile(DuckObject obj, DuckObject other)
    {
        var factionComp = obj.GetComp<Faction>();
        var otherFactionComp = other.GetComp<Faction>();
        // TODO: add faction relations
        return factionComp.FactionId != otherFactionComp.FactionId;
    }

    public static bool LetPass(DuckObject obj, DuckObject other)
    {
        var factionComp = obj.GetComp<Faction>();
        return factionComp.FactionId == Enums.FactionId.Player && !IsHostile(obj, other);
    }
    
    public static Enums.FactionId GetFactionId(DuckObject obj)
    {
        var factionComp = obj.GetComp<Faction>();
        return factionComp.FactionId;
    }
}