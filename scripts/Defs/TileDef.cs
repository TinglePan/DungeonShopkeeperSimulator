using DSS.Common;
using DSS.Game.Components;

namespace DSS.Defs;

public class TileDef
{
    public Enums.TileId Id;
    public GlyphRenderable GlyphRenderable;
    
    public TileDef(Enums.TileId id, GlyphRenderable glyphRenderable)
    {
        Id = id;
        GlyphRenderable = glyphRenderable;
    }
}