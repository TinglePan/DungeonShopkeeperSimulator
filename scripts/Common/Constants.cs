using System;
using System.Collections.Generic;
using Godot;

namespace DSS.Common;

public static class Constants
{
	public static readonly Vector2I TileSize = new Vector2I(8, 8);
	public static readonly int HoldTime = 500;
	public static readonly string MonoTileSetDefPath = "res://defs/minirogue-c64-all.json";
	// NOTE: This is the same as MonoTileSetDefPath without the extension.
	public static readonly string MonoTileSetDefStrId = "minirogue-c64-all";
	public static readonly string MonoTileSetAtlasPath = "res://textures/minirogue-c64-all.png";
	public static readonly string TileDefPath = "res://defs/tiles.json";
}
