using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class PlayerControl: BaseComp
{
    public Enums.InputSource Player;
    public bool IsActivated;

    public static void Setup(Game game, Entity obj, Enums.InputSource player, bool isActivated)
    {
        var playerControlComp = obj.GetCompOrNew<PlayerControl>();
        playerControlComp.GameRef = game;
        playerControlComp.Player = player;
        playerControlComp.IsActivated = isActivated;
    }

    public bool IsCurrentlyControlledBy(Enums.InputSource player=default) 
    {
        var res = IsActivated;
        if (player != default)
        {
            res &= Player == player;
        }
        return res;
    }
}