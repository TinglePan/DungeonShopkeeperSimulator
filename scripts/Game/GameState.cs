namespace DSS.Game;

public class GameState
{
    private static GameState _instance;

    public static GameState Instance => _instance ??= new GameState();

    public Ecs.Ecs World;
    public GameHandlerBase CurrentGameHandler;
}