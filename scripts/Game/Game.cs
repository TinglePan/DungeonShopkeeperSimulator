namespace DSS.Game;

public class Game
{
    private static Game _instance;
    public static Game Instance => _instance ??= new Game();

    public GameState GameState { get; }
    public DefStore DefStore { get; }
    
    public Game()
    {
        GameState = new GameState();
        GameState.Init();
        DefStore = new DefStore();
        DefStore.Init();
    }
}