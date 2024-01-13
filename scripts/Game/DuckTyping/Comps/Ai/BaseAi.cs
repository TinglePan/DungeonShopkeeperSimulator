using System.Collections.Generic;
using DSS.Game.Actions;
using Godot;

namespace DSS.Game.DuckTyping.Comps.Ai;

public abstract class BaseAi: BaseComp
{
    protected DuckObject CreatureRef;

    protected static void Setup(DuckObject obj)
    {
        var aiComp = obj.GetCompOrNew<ChaseAi>();
        aiComp.CreatureRef = obj;
    }

    public abstract void Step(ActionManager actionManager);
}

