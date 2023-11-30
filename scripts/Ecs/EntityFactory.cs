using DSS.Ecs.Components;

namespace DSS.Ecs;

public class EntityFactory
{
    public static Entity CreateEntity(Ecs ecs)
    {
        var components = new ComponentBase[]
        {
            new Coord(0, 0)
        };
        var entity = ecs.CreateEntity(components);
        
        
        return entity;
    }
}