using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Rotatable: BaseComp
{
    public Enums.Direction4 Direction;
    public Vector2I Pivot;
}