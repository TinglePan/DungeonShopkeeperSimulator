using System;
using DSS.Common;
using Godot;

namespace DSS.Ecs.Components;

public class Renderable: ComponentBase
{
}

public class GlyphRenderable : Renderable
{
	public string Glyph;
	public string FontName;
	public Color FgColor;
	public Color BgColor;
	
	public GlyphRenderable(string glyph, string fontName, Color fgColor, Color bgColor)
	{
		Glyph = glyph;
		FontName = fontName;
		FgColor = fgColor;
		BgColor = bgColor;
	}
}

public class SpriteRenderable : Renderable
{
	public string SpritePath;
	public string MaterialName;
	
	public SpriteRenderable(string spritePath, string materialName)
	{
		SpritePath = spritePath;
		MaterialName = materialName;
	}
}