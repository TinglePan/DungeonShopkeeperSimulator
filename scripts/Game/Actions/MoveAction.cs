using System;
using System.Diagnostics;
using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.Actions;

public class MoveAction: ActionToCoord
{
    public MoveAction(DuckObject entity, Vector2I targetCoord, Map map=null): base(entity, targetCoord, map)
    {
    }
    
    protected override bool TryPerform()
    {
        return MapRef.MoveObject(EntityRef, TargetCoord);
    }
}