using DSS.Common;
using DSS.Defs;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;
using ImpromptuInterface;

namespace DSS.Game;

public partial class CreatureController: Node2D
{
    [Export] public Sprite2D SpriteRef;
    private DuckObject _creature;

    public void Init(DuckObject creature)
    {
        _creature = creature;
        OnMap.RegisterCoordChangeCallback(creature, OnCoordChanged);
        if (TileRenderable.CheckDuckType(creature))
        {
            AtlasTexture texture = new AtlasTexture();
            var texturePath = Renderable.GetAtlasPath(creature);
            texture.Atlas = GD.Load<Texture2D>(texturePath);
            var def = Game.Instance.DefStore.GetDef<TileAtlasDef>(Utils.TexturePathToDefPath(texturePath));
            var tileCoord = TileRenderable.GetTileCoord(creature, def);
            texture.Region = new Rect2(tileCoord.X * def.TileSize.X, tileCoord.Y * def.TileSize.Y,
                def.TileSize.X, def.TileSize.Y);
            SpriteRef.Texture = texture;
        } else if (SpriteRenderable.CheckDuckType(creature))
        {
            var texturePath = Renderable.GetAtlasPath(creature);
            Texture2D texture = GD.Load<Texture2D>(texturePath);
            SpriteRef.Texture = texture;
        }
        var coord = OnMap.GetCoord(creature);
        Position = coord * Constants.TileSize;
        SpriteRef.Offset = SpriteRef.Texture.GetSize() / 2;
        SpriteRef.ZIndex = (int)Enums.ObjectRenderOrder.Creature;
    }
    
    public void OnCoordChanged(Vector2I fromCoord, Vector2I toCoord)
    {
        Position = toCoord * Constants.TileSize;
    }
    
}