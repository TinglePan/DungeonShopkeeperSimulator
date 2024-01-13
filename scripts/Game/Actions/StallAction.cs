﻿using System;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.Actions;

public class StallAction: BaseAction
{
    protected DuckObject EntityRef;
    protected Map MapRef;
    
    public StallAction(DuckObject entity, Map map=null)
    {
        EntityRef = entity;
        MapRef = map ?? OnMap.GetMap(entity);
    }
    
    protected override bool TryPerform()
    {
        GD.Print("Stall");
        return true;
    }
}