using System;
using System.Collections.Generic;
using DSS.Game.Components;
using Godot;

namespace DSS.Common;

public static class Constants
{
	public static readonly Vector2 TileSize = new (16, 16);
	public static readonly Vector2 GlyphAtlasSize = new (16, 16);
	public static readonly string GlyphAtlasPath = "res://textures/glyphs.png";
	public static readonly Enums.RenderMode RenderMode = Enums.RenderMode.Glyph;

	public static readonly string GlyphSet =
		"!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
}
