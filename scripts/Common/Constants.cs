using System;
using System.Collections.Generic;
using Godot;

namespace DSS.Common;

public static class Constants
{
	public static readonly Vector2I TileSize = new Vector2I(8, 8);
	public const int HoldTime = 500;

	public const string MonoTileSetDefPath = "res://defs/minirogue-c64-all.json";
	// NOTE: This is the same as MonoTileSetDefPath without the extension.
	public const string MonoTileSetDefStrId = "minirogue-c64-all";
	public const string MonoTileSetAtlasPath = "res://textures/minirogue-c64-all.png";
	public const string TilemapVisibilityOverlayDefPath = "res://defs/tilemap_visibility_overlay.json";
	public const string TilemapVisibilityOverlayDefStrId = "tilemap_visibility_overlay";
	public const string TilemapVisibilityOverlayAtlasPath = "res://textures/tilemap_visibility_overlay.png";
	public const string TileDefPath = "res://defs/tiles.json";
	
	public const string EntityTagCreature = "creature";
	public const string EntityTagWall = "wall";
	public const string EntityTagFurniture = "furniture";
	public const string EntityTagItem = "item";
	
}
