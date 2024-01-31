using System;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.Actions;

public class StallAction: BaseAction
{
    
    public StallAction(Game game, Entity entity): base(game, entity)
    {
    }
    
    public override bool TryPerform()
    {
        GD.Print("Stall");
        return true;
    }
}