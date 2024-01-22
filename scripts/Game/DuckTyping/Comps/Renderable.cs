using System;
using System.Text.Json.Serialization;
using DSS.Common;
using DSS.Defs;
using Godot;
using Microsoft.VisualBasic;

namespace DSS.Game.DuckTyping.Comps;

public class Renderable: BaseComp
{
	public string AtlasPath;
	public bool HideWhenNotVisible;
	
	[JsonIgnore]
	public Watched<bool> IsVisible = new Watched<bool>(true);
	
	public static void Setup(DuckObject obj, string atlasPath, bool hideWhenNotVisible=false)
	{
		var renderableComp = obj.GetCompOrNew<Renderable>();
		renderableComp.AtlasPath = atlasPath;
		renderableComp.HideWhenNotVisible = hideWhenNotVisible;
	}

	public static string GetAtlasPath(DuckObject obj)
	{
		var renderableComp = obj.GetComp<Renderable>();
		return renderableComp.AtlasPath;
	}
	
	public static void ToggleVisibility(DuckObject obj, bool isVisible)
	{
		var renderableComp = obj.GetComp<Renderable>();
		renderableComp.IsVisible.Value = isVisible;
	}
}

public class SpriteRenderable : Renderable
{
	public string MaterialName;
	
	public static void Setup(DuckObject obj, string atlasPath, string materialName, bool hideWhenNotVisible=false)
	{
		var renderableComp = obj.GetCompOrNew<SpriteRenderable>();
		Renderable.Setup(obj, atlasPath, hideWhenNotVisible);
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
	
	public static void Setup(DuckObject obj, string atlasPath, int tileId, bool hideWhenNotVisible=false)
	{
		var renderableComp = obj.GetCompOrNew<TileRenderable>();
		Renderable.Setup(obj, atlasPath, hideWhenNotVisible);
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