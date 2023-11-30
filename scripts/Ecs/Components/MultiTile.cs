using Godot;

namespace DSS.Ecs.Components;

public class MultiTile: ComponentBase
{
    public int Width;
    public int Height;
    public Vector2I Pivot;
    
    public MultiTile(int width, int height, Vector2I pivot)
    {
        Width = width;
        Height = height;
        Pivot = pivot;
    }
}