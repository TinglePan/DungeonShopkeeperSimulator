using System;
using DSS.Common;
using Godot;

namespace DSS.Game.Components;

public record Renderable
{
}

public record GlyphRenderable : Renderable
{
	public char Glyph;
	public Color FgColor;
	public Color BgColor;
	
	public GlyphRenderable(char glyph, Color fgColor, Color bgColor)
	{
		Glyph = glyph;
		FgColor = fgColor;
		BgColor = bgColor;
	}
}

public record SpriteRenderable : Renderable
{
	public string SpritePath;
	public string MaterialName;
	
	public SpriteRenderable(string spritePath, string materialName)
	{
		SpritePath = spritePath;
		MaterialName = materialName;
	}
}