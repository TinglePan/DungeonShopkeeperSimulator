using System;
using System.Collections.Generic;

namespace DSS.Game;

public class GameState
{
    public GameSetting Setting;
    public Random Rand;
    public BaseGameHandler CurrentGameHandler;
    public Map CurrentMap;
    public Dictionary<Guid, Map> Maps;
    public int CurrentTurn;
    
    public void Init()
    {
        Setting = new GameSetting();
        Setting.Init();
        Rand = new Random((int)DateTime.Now.Ticks);
        Maps = new Dictionary<Guid, Map>();
    }

    public void OnPlayerTurnEnd()
    {
        
    }
    
    public void OnPublicTurnEnd()
    {
        
    }
}