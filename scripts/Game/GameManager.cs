using System;
using DSS.Common;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game;

public partial class GameManager : Node2D
{
	[Export] public MapController MapController;
	public Game Game;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Game = Game.Instance;
		// EcsStore = EcsStore.Instance;
		// EcsStore.Init();

		// var map = new Map(EcsStore, new Vector2I(40, 40));
		// GameState.CurrentMap = map;
		// BaseEntity player = new BaseEntity(EcsStore);
		// player.AddComp(new TileRenderable(EcsStore, Constants.MonoTileSetAtlasPath, new Vector2I(3, 7)));
		// GameState.Player = player;
		// EcsStore.GetSystem<MapSystem>().SpawnEntity(map, Enums.MapEntityCategory.Creature, player, new Vector2I(10, 10));
		//
		// MapGen gen = new MapGen();
		// gen.DungeonMaze(GameState.CurrentMap);
		// MapRenderer.Init(GameState.CurrentMap);
		//
		// GameState.CurrentGameHandler = new MapExploreHandler();
		// TileMapController.Init(GameState.World.GetEntity());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		Game.GameState.CurrentGameHandler.Step(@event);
	}
}
