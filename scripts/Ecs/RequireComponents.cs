using GoRogue.Components;

namespace DSS.Ecs;

[System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct)
]
public class RequireComponents : System.Attribute
{
    public ComponentTagPair[] CompIds;

    public RequireComponents(params ComponentTagPair[] compIds)
    {
        CompIds = compIds;
    }
}