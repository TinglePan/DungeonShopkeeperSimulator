using DSS.Common;

namespace DSS.Ecs.Components;

public class Rotatable: ComponentBase
{
    public Enums.Direction4 Direction;
    
    public Rotatable(Enums.Direction4 direction)
    {
        Direction = direction;
    }
}