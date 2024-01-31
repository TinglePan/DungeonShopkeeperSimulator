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
	public bool HideUnseen;
	public Watched<bool> IsVisible = new (true);
	
	public static void Setup(Game game, Entity obj, string atlasPath, bool hideUnseen=false)
	{
		var renderableComp = obj.GetCompOrNew<Renderable>();
		renderableComp.GameRef = game;
		renderableComp.EntityRef = obj;
		renderableComp.AtlasPath = atlasPath;
		renderableComp.HideUnseen = hideUnseen;
	}

	public void UpdateVisibility()
	{
		if (HideUnseen && EntityRef.GetComp<OnMap>(ensure: false) is { } onMapComp)
		{
			var map = onMapComp.Map;
			var coord = onMapComp.Coord.Value;
			if (!map.VisibleTiles[map.CoordToIndex(coord)])
			{
				IsVisible.Value = false;
				return;
			}
		}
		IsVisible.Value = true;
	}
}

public class SpriteRenderable : Renderable
{
	public string MaterialName;
	
	public static void Setup(Game game, Entity obj, string atlasPath, string materialName, bool hideUnseen=false)
	{
		var renderableComp = obj.GetCompOrNew<SpriteRenderable>();
		Renderable.Setup(game, obj, atlasPath, hideUnseen);
		renderableComp.MaterialName = materialName;
	}
}

public class TileRenderable: Renderable
{
	public Vector2I Offset;
	
	public static void Setup(Game game, Entity obj, string atlasPath, Vector2I offset, bool hideUnseen=false)
	{
		var renderableComp = obj.GetCompOrNew<TileRenderable>();
		Renderable.Setup(game, obj, atlasPath, hideUnseen);
		renderableComp.Offset = offset;
	}
}