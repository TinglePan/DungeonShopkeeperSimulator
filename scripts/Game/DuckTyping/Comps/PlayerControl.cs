using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class PlayerControl: BaseComp
{
    public Enums.InputSource Player;
    public bool IsActivated;

    public static void Setup(DuckObject obj, Enums.InputSource player, bool isActivated)
    {
        var playerControlComp = obj.GetCompOrNew<PlayerControl>();
        playerControlComp.Player = player;
        playerControlComp.IsActivated = isActivated;
    }

    public static bool IsCurrentlyControlledBy(DuckObject obj, Enums.InputSource player=default) 
    {
        var playerControlComp = obj.GetComp<PlayerControl>();
        var res = playerControlComp.IsActivated;
        if (player != default)
        {
            res &= playerControlComp.Player == player;
        }
        return res;
    }
}