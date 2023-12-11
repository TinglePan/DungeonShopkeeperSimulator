using System;
using System.Diagnostics;
using System.Net.Mime;
using DSS.Common;
using Godot;

namespace DSS.Game;

public partial class TileMapController : Node2D
{
    [Export]
    public Sprite2D SpriteRef;

    public Map MapData;
    private Image _mapAtlas;
    private Image _mapFgColorAtlas;
    private Image _mapBgColorAtlas;
    
    public void Init(Map map)
    {
        MapData = map;
        var texture = ImageTexture.CreateFromImage(
            Image.Create(map.MultiTile.Width * (int)Constants.TileSize.X,
                map.MultiTile.Height * (int)Constants.TileSize.Y, false, Image.Format.R8));
        _mapAtlas = Image.Create(MapData.MultiTile.Width, MapData.MultiTile.Height, false,
            Image.Format.Rgba8);
        _mapFgColorAtlas = Image.Create(MapData.MultiTile.Width, MapData.MultiTile.Height, false,
            Image.Format.Rgba8);
        _mapBgColorAtlas = Image.Create(MapData.MultiTile.Width, MapData.MultiTile.Height, false,
            Image.Format.Rgba8);
        SpriteRef.Texture = texture;
        Render();
    }

    public void Render()
    {
        GenerateMapAtlas(GameState.Instance.Setting.RenderMode);
        switch (GameState.Instance.Setting.RenderMode)
        {
            case Enums.RenderMode.Glyph:
                SpriteRef.Material.Set("shader_parameter/tileSize", Constants.TileSize);
                SpriteRef.Material.Set("shader_parameter/mapSize", 
                    new Vector2I(MapData.MultiTile.Width, MapData.MultiTile.Height));
                var mapAtlas = ImageTexture.CreateFromImage(_mapAtlas);
                var mapFgColorAtlas = ImageTexture.CreateFromImage(_mapFgColorAtlas);
                var mapBgColorAtlas = ImageTexture.CreateFromImage(_mapBgColorAtlas);
                SpriteRef.Material.Set("shader_parameter/mapAtlas", mapAtlas);
                SpriteRef.Material.Set("shader_parameter/mapFgColorAtlas", mapFgColorAtlas);
                SpriteRef.Material.Set("shader_parameter/mapBgColorAtlas", mapBgColorAtlas);
                SpriteRef.Material.Set("shader_parameter/glyphAtlasSize", Constants.GlyphAtlasSize);
                Texture2D glyphAtlas = ResourceLoader.Load<Texture2D>(Constants.GlyphAtlasPath);
                SpriteRef.Material.Set("shader_parameter/glyphAtlas", glyphAtlas);
                break;
            case Enums.RenderMode.TileSet:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void GenerateMapAtlas(Enums.RenderMode renderMode)
    {
        // Draw image
        
        for (int x = 0; x < MapData.MultiTile.Width; x++)
        {
            for (int y = 0; y < MapData.MultiTile.Height; y++)
            {
                var index = MapData.CoordToIndex(new Vector2I(x, y));
                Debug.Assert(index < MapData.Size);
                var view = MapData.View;
                var tileId = view[index];
                var glyphRenderable = DefStore.Instance.TileDefs[(Enums.TileId)tileId].GlyphRenderable;
                var glyphId = DefStore.Instance.GlyphAtlasDef.GlyphMap[glyphRenderable.Glyph];
                var fgColor = glyphRenderable.FgColor;
                var bgColor = glyphRenderable.BgColor;
                Color color = Color.Color8((byte)glyphId, 0, 0, 0);
                _mapAtlas.SetPixel(x, y, color);
                _mapFgColorAtlas.SetPixel(x, y, fgColor);
                _mapBgColorAtlas.SetPixel(x, y, bgColor);
            }
        }
    }
}