using System;
using DSS.Common;
using DSS.Defs;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Renderable: BaseComp
{
	public string AtlasPath;
	
	public static bool CheckDuckType(DuckObject obj)
	{
		return obj.GetComp<Renderable>() != null;
	}
	
	public static void Setup(DuckObject obj, string atlasPath)
	{
		var renderableComp = new Renderable
		{
			AtlasPath = atlasPath,
		};
		obj.AddComp(renderableComp);
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
	
	public new static bool CheckDuckType(DuckObject obj)
	{
		return obj.GetComp<SpriteRenderable>() != null;
	}
	
	public static void Setup(DuckObject obj, string atlasPath, string materialName)
	{
		var renderableComp = new SpriteRenderable
		{
			AtlasPath = atlasPath,
			MaterialName = materialName,
		};
		obj.AddComp(renderableComp);
	}
	
	public static string GetMaterialName(DuckObject obj)
	{
		var renderableComp = obj.GetComp<SpriteRenderable>();
		return renderableComp.MaterialName;
	}
}

public class TileRenderable: Renderable
{
	public Enums.TileId TileId;
	
	public new static bool CheckDuckType(DuckObject obj)
	{
		return obj.GetComp<TileRenderable>() != null;
	}
	
	public static void Setup(DuckObject obj, string atlasPath, Enums.TileId tileId)
	{
		var renderableComp = new TileRenderable
		{
			AtlasPath = atlasPath,
			TileId = tileId
		};
		obj.AddComp(renderableComp);
	}
	
	public static Enums.TileId GetTileId(DuckObject obj)
	{
		var renderableComp = obj.GetComp<TileRenderable>();
		return renderableComp.TileId;
	}
	
	public static Vector2I GetTileCoord(DuckObject obj, TileAtlasDef def)
	{
		return def.TileIdToCoordMap[GetTileId(obj)];
	}
}