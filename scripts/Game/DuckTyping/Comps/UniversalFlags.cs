using System.Collections.Generic;
using DSS.Common;

namespace DSS.Game.DuckTyping.Comps;

public class UniversalFlags: BaseComp
{
    public Enums.EntityFlag Flags;
    
    public static void Setup(Game game, Entity obj, params Enums.EntityFlag[] flags)
    {
        var universalFlagsComp = obj.GetCompOrNew<UniversalFlags>();
        universalFlagsComp.GameRef = game;
        universalFlagsComp.EntityRef = obj;
        foreach (var flag in flags)
        {
            universalFlagsComp.Flags |= flag;
        }
    }
    
    public bool HasFlags(params Enums.EntityFlag[] flags)
    {
        Enums.EntityFlag checkValue = 0;
        foreach (var flag in flags)
        {
            checkValue |= flag;
        }
        return (Flags & checkValue) == checkValue;
    }
}