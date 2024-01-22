using DSS.Common;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using DSS.Game.DuckTyping.Comps.Ai;
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

		var map = Game.GenLevel();
		var mapNode = MapPrefab.Instantiate<Node2D>();
		mapNode.Name = "Map";
		AddChild(mapNode);
		MapController = mapNode as MapController;
		MapController?.Init(map);

		DuckObject player = new DuckObject();
		TileRenderable.Setup(player, Constants.MonoTileSetAtlasPath, Game.DefStore.GetTileId("player"));
		Faction.Setup(player, Enums.FactionId.Player);
		PlayerViewshed.Setup(player, 50, 180);
		FaceDir.Setup(player, Enums.Direction8.Up);
		map.SpawnObject(player, new Vector2I(10, 10), Enums.DuckObjectTag.Creature);
		
		var playerNode = PlayerPrefab.Instantiate<Node2D>();
		playerNode.Name = "Player";
		AddChild(playerNode);
		PlayerController = playerNode as CreatureController;
		PlayerController?.Init(player);

		DuckObject testMonster = new DuckObject();
		TileRenderable.Setup(testMonster, Constants.MonoTileSetAtlasPath, Game.DefStore.GetTileId("adventurer"),
			hideWhenNotVisible:true);
		Faction.Setup(testMonster, Enums.FactionId.Adventurer);
		Viewshed.Setup(testMonster, 5, 360);
		ChaseAi.Setup(testMonster, player);
		map.SpawnObject(testMonster, new Vector2I(20, 20), Enums.DuckObjectTag.Creature);
		
		var testMonsterNode = PlayerPrefab.Instantiate<Node2D>();
		testMonsterNode.Name = "Adventurer";
		AddChild(testMonsterNode);
		var testMonsterController = testMonsterNode as CreatureController;
		testMonsterController?.Init(testMonster);
		
		
		Game.GameState.CurrentGameHandler = new MapExploreHandler(Game, player);
		Game.GameState.CurrentGameHandler.OnEnter();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Game.InputManager.OnPhysicsProcess();
	}

	public override void _Input(InputEvent @event)
	{
		Game.InputManager.OnInput(@event);
	}
}
