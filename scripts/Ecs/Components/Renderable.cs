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
}

public class SpriteRenderable : Renderable
{
	public string SpritePath;
	public string MaterialName;
}