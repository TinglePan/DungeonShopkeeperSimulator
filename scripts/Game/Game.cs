using DSS.Game.DuckTyping.Comps.Ai;
using Godot;

namespace DSS.Game;

public class Game
{
    private static Game _instance;
    public static Game Instance => _instance ??= new Game();

    public GameState GameState { get; }
    public DefStore DefStore { get; }
    public InputManager InputManager { get; }
    public ActionManager ActionManager { get; }
    
    public Game()
    {
        GameState = new GameState();
        GameState.Init();
        DefStore = new DefStore();
        DefStore.Init();
        InputManager = new InputManager();
        InputManager.Init();
        ActionManager = new ActionManager();
        ActionManager.Init(this);
    }

    public Map GenLevel()
    {
        var map = new Map(new Vector2I(40, 40));
        MapGen gen = new MapGen();
        gen.DungeonMaze(this, map);
        GameState.Maps.Add(map.Id, map);
        GameState.CurrentMap = map;
        return map;
    }

    public void PlayerTurnEnd()
    {
        GameState.OnPlayerTurnEnd();
        HandleCreatureAi();
        PublicTurnEnd();
    }
    
    public void PublicTurnEnd()
    {
        GameState.OnPublicTurnEnd();
    }

    protected void HandleCreatureAi()
    {
        foreach (var creature in GameState.CurrentMap.Creatures)
        {
            if (creature.GetComp<BaseAi>(ensure:false) is { } ai)
            {
                ai.Step(ActionManager);
            }
        }
    }
}