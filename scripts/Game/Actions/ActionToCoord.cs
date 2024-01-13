using System;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.Actions;

public abstract class ActionToCoord: BaseAction
{
    protected Map MapRef;
    protected DuckObject EntityRef;
    protected Vector2I TargetCoord;
    
    protected ActionToCoord(DuckObject entity, Vector2I targetCoord, Map map=null)
    {
        EntityRef = entity;
        MapRef = map ?? OnMap.GetMap(entity);
        TargetCoord = targetCoord;
    }
}