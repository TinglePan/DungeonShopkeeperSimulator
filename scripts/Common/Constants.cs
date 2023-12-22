using System;
using System.Collections.Generic;
using Godot;

namespace DSS.Common;

public static class Constants
{
	public static readonly Vector2 TileSize = new (16, 16);
	public static readonly string MonoTileSetDefPath = "res://defs/minirogue-c64-all.json";
	public static readonly string MonoTileSetAtlasPath = "res://textures/minirogue-c64-all.png";
}
