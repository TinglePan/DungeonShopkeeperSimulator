namespace DSS.Ecs.Components;

public class Viewshed: ComponentBase
{
    public int Range;
    public int Angle;
    
    public Viewshed(int range, int angle)
    {
        Range = range;
        Angle = angle;
    }
}