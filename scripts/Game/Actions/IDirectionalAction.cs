using DSS.Common;

namespace DSS.Game.Actions;

public interface IDirectionalAction
{
    public Enums.Direction8 Dir
    {
        get;
        set;
    }
}