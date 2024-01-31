using System.Collections.Generic;
using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Faction: BaseComp
{
    public Enums.FactionId FactionId;

    public static void Setup(Game game, Entity obj, Enums.FactionId factionId)
    {
        var factionComp = obj.GetCompOrNew<Faction>();
        factionComp.GameRef = game;
        factionComp.EntityRef = obj;
        factionComp.FactionId = factionId;
    }

    public bool IsHostile(Faction other)
    {
        // TODO: add faction relations
        return FactionId != other.FactionId;
    }
}