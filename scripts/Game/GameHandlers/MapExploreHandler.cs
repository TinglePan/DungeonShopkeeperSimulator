using System.Collections.Generic;
using System.Reflection.Metadata;
using DSS.Common;
using DSS.Game.Actions;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game;

public class MapExploreHandler: BaseGameHandler
{
    private DuckObject _player;
    
    public MapExploreHandler(Game game, DuckObject player) : base(game)
    {
        _player = player;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Game.Instance.InputManager.OnDirectionInput += OnDirectionInput;
        Game.Instance.InputManager.OnHoldDirectionInput += OnDirectionInput;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void OnDirectionInput(Enums.Direction9 dir)
    {
        var map = OnMap.GetMap(_player);
        var dxy = map.DirToDxy(dir);
        var targetCoord = OnMap.GetCoord(_player) + dxy;
        var action = map.TryMoveObject(_player, targetCoord);
        // TODO: Action point based turn change
        GameRef.ActionManager.Perform(action, onSuccess: GameRef.PlayerTurnEnd);
    }
}