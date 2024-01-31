using System.Collections.Generic;
using DSS.Game.Actions;
using Godot;

namespace DSS.Game.DuckTyping.Comps.Ai;

public abstract class BaseAi: BaseComp
{
    protected static void Setup(Game game, Entity obj)
    {
        var aiComp = obj.GetCompOrNew<ChaseAi>();
        aiComp.GameRef = game;
        aiComp.EntityRef = obj;
    }

    public abstract void Step(ActionManager actionManager);
}

