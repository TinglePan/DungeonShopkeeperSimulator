using DSS.Common;
using DSS.Game.DuckTyping;

namespace DSS.Game.Actions;

public class TurnAroundAction: BaseAction
{
    protected DuckObject EntityRef;
    public Enums.Direction8 Dir;
    
    public TurnAroundAction(DuckObject entity, Enums.Direction8 dir)
    {
        EntityRef = entity;
        Dir = dir;
    }
    protected override bool TryPerform()
    {
        throw new System.NotImplementedException();
    }
}