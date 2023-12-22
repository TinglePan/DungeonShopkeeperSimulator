using DSS.Game.DuckTyping;
using Godot;

namespace DSS.Game.Actions;

public class StallAction: BaseAction
{
    protected Map MapRef;
    protected DuckObject EntityRef;
    
    public StallAction(Map map, DuckObject entity)
    {
        MapRef = map;
        EntityRef = entity;
    }
    
    public override bool Execute()
    {
        GD.Print("Stall");
        return true;
    }
}