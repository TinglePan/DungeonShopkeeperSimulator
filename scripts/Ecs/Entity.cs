using System;
using System.Collections.Generic;
using DSS.Ecs.Components;
using GoRogue.Components;

namespace DSS.Ecs;

public class Entity
{
    public Guid Id;
    public HashSet<Guid> ComponentIds;
    public Entity(ComponentBase[] components=null)
    {
        ComponentIds = new HashSet<Guid>();
        if (components != null)
        {
            foreach (var comp in components)
            {
                ComponentIds.Add(comp.Id);
                comp.EntityIds.Add(Id);
            }
        }
    }
    
}