using System.Collections.Generic;
using DSS.Common;
using DSS.Defs;
using DSS.Game.Components;
using Godot;

namespace DSS.Game;

public class DefStore
{
    private static DefStore _instance;
    public static DefStore Instance => _instance ??= new DefStore();
    
    public Dictionary<Enums.TileId, TileDef> TileDefs = new Dictionary<Enums.TileId, TileDef>();
    public GlyphAtlasDef GlyphAtlasDef = new GlyphAtlasDef();

    public void Load()
    {
        TileDefs.Add(Enums.TileId.Floor, new TileDef(
            Enums.TileId.Floor,
            new GlyphRenderable(
                '.',
                Colors.Black,
                Colors.White
            )));
        TileDefs.Add(Enums.TileId.Wall, new TileDef(
            Enums.TileId.Wall,
            new GlyphRenderable(
                '#',
                Colors.Black,
                Colors.Green
            )));
        
        for (int i = 0; i < Constants.GlyphSet.Length; i++)
        {
            var c = Constants.GlyphSet[i];
            GlyphAtlasDef.GlyphMap.Add(c, i);
        }
    }
}