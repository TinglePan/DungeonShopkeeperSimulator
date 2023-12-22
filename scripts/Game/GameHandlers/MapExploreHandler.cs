using System.Collections.Generic;
using System.Reflection.Metadata;
using DSS.Common;
using DSS.Game.Actions;
using DSS.Game.DuckTyping;
using Godot;

namespace DSS.Game;

public class MapExploreHandler: BaseGameHandler
{
    private DuckObject _player;
    
    public MapExploreHandler(Game game, DuckObject player) : base(game)
    {
        _player = player;
    }

    public override IEnumerable<BaseAction> HandleInput(InputEvent @event)
    {
        var map = GameRef.GameState.CurrentMap;
        
        Enums.Direction9 dir = 0;
        if (@event.IsActionPressed("Stall"))
        {
            yield return map.TryMoveObject(_player, dir);
        }
        if (@event.IsActionPressed("UpLeft"))
        {
            dir |= Enums.Direction9.UpLeft;
        }
        if (@event.IsActionPressed("UpRight"))
        {
            dir |= Enums.Direction9.UpRight;
        }
        if (@event.IsActionPressed("DownLeft"))
        {
            dir |= Enums.Direction9.DownLeft;
        }
        if (@event.IsActionPressed("DownRight"))
        {
            dir |= Enums.Direction9.DownRight;
        }
        if (@event.IsActionPressed("Right"))
        {
            dir |= Enums.Direction9.Right;
        }
        if (@event.IsActionPressed("Left"))
        {
            dir |= Enums.Direction9.Left;
        }
        if (@event.IsActionPressed("Up"))
        {
            dir |= Enums.Direction9.Up;
        }
        if (@event.IsActionPressed("Down"))
        {
            dir |= Enums.Direction9.Down;
        }
        if (dir != 0)
        {
            yield return map.TryMoveObject(_player, dir);
        }
        // TODO: Handle other inputs
    }

    public override BaseGameHandler Step(InputEvent @event)
    {
        var actions = HandleInput(@event);
        foreach (var action in actions)
        {
            action.Execute();
        }
        return this;
    }
}