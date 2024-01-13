using System;
using DSS.Common;
using DSS.Defs;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Renderable: BaseComp
{
	public string AtlasPath;
	
	public static void Setup(DuckObject obj, string atlasPath)
	{
		var renderableComp = obj.GetCompOrNew<Renderable>();
		renderableComp.AtlasPath = atlasPath;
	}

	public static string GetAtlasPath(DuckObject obj)
	{
		var renderableComp = obj.GetComp<Renderable>();
		return renderableComp.AtlasPath;
	}
}

public class SpriteRenderable : Renderable
{
	public string MaterialName;
	
	public static void Setup(DuckObject obj, string atlasPath, string materialName)
	{
		var renderableComp = obj.GetCompOrNew<SpriteRenderable>();
		renderableComp.AtlasPath = atlasPath;
		renderableComp.MaterialName = materialName;
	}
	
	public static string GetMaterialName(DuckObject obj)
	{
		var renderableComp = obj.GetComp<SpriteRenderable>();
		return renderableComp.MaterialName;
	}
}

public class TileRenderable: Renderable
{
	public int TileId;
	
	public static void Setup(DuckObject obj, string atlasPath, int tileId)
	{
		var renderableComp = obj.GetCompOrNew<TileRenderable>();
		renderableComp.AtlasPath = atlasPath;
		renderableComp.TileId = tileId;
	}
	
	public static int GetTileId(DuckObject obj)
	{
		var renderableComp = obj.GetComp<TileRenderable>();
		return renderableComp.TileId;
	}
	
	public static Vector2I GetTileCoord(DuckObject obj, TileAtlasDef def)
	{
		return def.TileIdToCoordMap[GetTileId(obj)];
	}
}