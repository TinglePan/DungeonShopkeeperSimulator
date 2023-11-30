using System;
using System.Collections.Generic;

namespace DSS.Ecs.Components;

public class ComponentBase
{
    public Guid Id = Guid.NewGuid();
    public HashSet<Guid> EntityIds = new();
}