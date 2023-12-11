using System;
using DSS.Common;
using Godot;

namespace DSS.Game;

public partial class GameManager : Node2D
{
	[Export] public TileMapController TileMapController;
	public GameState GameState;
	public DefStore DefStore;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameState = GameState.Instance;
		DefStore = DefStore.Instance;
		DefStore.Load();
		var map = new Map(new Vector2I(10, 10));
		map.MultiTile.Tiles[4] = (UInt16)Enums.TileId.Wall;
		GameState.CurrentMap = map;
		TileMapController.Init(GameState.CurrentMap);
		// TileMapController.Init(GameState.World.GetEntity());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
