using System;
using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Renderable: BaseComp
{
	public string AtlasPath;
}

public class SpriteRenderable : Renderable
{
	public string MaterialName;
	public Dimension IntendedDimension;
}

public class TileRenderable: Renderable
{
	public Vector2I TileCoord;
}