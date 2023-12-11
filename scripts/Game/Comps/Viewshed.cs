namespace DSS.Game.Components;

public record Viewshed
{
    public int Range;
    public int Angle;
    
    public Viewshed(int range, int angle)
    {
        Range = range;
        Angle = angle;
    }
}