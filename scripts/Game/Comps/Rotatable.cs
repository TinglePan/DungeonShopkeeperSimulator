using DSS.Common;
using Godot;

namespace DSS.Game.Components;

public record Rotatable
{
    public Enums.Direction4 Direction;
    public Vector2I Pivot;
    
    public Rotatable(Enums.Direction4 direction, Vector2I pivot)
    {
        Direction = direction;
        Pivot = pivot;
    }
}