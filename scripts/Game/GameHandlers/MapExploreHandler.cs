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
    private Map _map;
    
    public MapExploreHandler(Game game, Map map) : base(game)
    {
        _map = map;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        GameRef.InputManager.OnDirectionInput += OnDirectionInput;
        GameRef.InputManager.OnHoldDirectionInput += OnDirectionInput;
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void OnDirectionInput(Enums.Direction9 dir)
    {
        
        if (dir == Enums.Direction9.Neutral)
        {
            new StallAction(GameRef, _map.PlayerControllingCreature).Perform();
        }
        else
        {
            new BumpAction(GameRef, _map.PlayerControllingCreature, (Enums.Direction8)dir).Perform();
        }
        // TODO: Action point based turn change
        GameRef.PlayerTurnEnd();
    }
}