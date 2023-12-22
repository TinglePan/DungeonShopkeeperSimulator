
using DSS.Common;
using Godot;

namespace DSS.Game.Actions;

public abstract class ActionWithDirection: BaseAction
{
    protected Enums.Direction9 Dir;
    protected ActionWithDirection(Enums.Direction9 dir)
    {
        Dir = dir;
    }
}