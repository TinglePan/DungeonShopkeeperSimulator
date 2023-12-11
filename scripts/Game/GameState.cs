namespace DSS.Game;

public class GameState
{
    private static GameState _instance;
    public static GameState Instance => _instance ??= new GameState();

    public GameSetting Setting = new GameSetting();
    public GameHandlerBase CurrentGameHandler;
    public Map CurrentMap;
}