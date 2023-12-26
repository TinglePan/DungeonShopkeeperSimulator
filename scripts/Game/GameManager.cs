using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game;

public partial class GameManager : Node2D
{
	[Export] public PackedScene MapPrefab;
	[Export] public PackedScene PlayerPrefab;
	public Game Game;
	public MapController MapController;
	public CreatureController PlayerController;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Game = Game.Instance;
		
		var map = new Map(new Vector2I(40, 40));
		Game.GameState.CurrentMap = map;

		MapGen gen = new MapGen();
		gen.DungeonMaze(map);

		DuckObject player = new DuckObject();
		TileRenderable.Setup(player, Constants.MonoTileSetAtlasPath, Enums.TileId.Player);
		map.SpawnObject(player, new Vector2I(10, 10), "Player");
		
		var mapNode = MapPrefab.Instantiate<Node2D>();
		mapNode.Name = "Map";
		AddChild(mapNode);
		MapController = mapNode as MapController;
		MapController?.Init(map);
		var playerNode = PlayerPrefab.Instantiate<Node2D>();
		playerNode.Name = "Player";
		AddChild(playerNode);
		PlayerController = playerNode as CreatureController;
		PlayerController?.Init(player);
		
		Game.GameState.CurrentGameHandler = new MapExploreHandler(Game, player);
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
