using Godot;
using System;
using DSS.Game;
using GoRogue.MapGeneration;
using GoRogue.Pathing;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace DSS;

public partial class Test : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// ViewportDemonstration.ExampleCode2();
		GD.Print(GameState.Instance);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

public static class ViewportDemonstration
{
	private static readonly Point Goal = (5, 6);

	public static void ExampleCode()
	{
		// Create a large view representing an entire map, from which we will use a subset
		// of values.  All of the edges will be obstacles; everything else will be clear.
		var largeView = new ArrayView<GoalState>(300, 300);
		foreach (var pos in largeView.Bounds().Expand(-1, -1).Positions())
			largeView[pos] = GoalState.Clear;

		// Add a goal
		largeView[Goal] = GoalState.Goal;

		// Create a viewport that will show a 10x10 area of the large view, starting at the
		// top left corner (position (0, 0))
		var viewport = new Viewport<GoalState>(
			largeView,
			new Rectangle(0, 0, 10, 10));

		// Create goal map using the viewport and calculate it.
		var goalMap = new GoalMap(viewport, Distance.Chebyshev);

		GD.Print("Initial Goal Map:");
		GD.Print(goalMap.ToString(5));

		// Move the viewport to the right and re-calculate the goal map
		viewport.SetViewArea(viewport.ViewArea.WithPosition((3, 0)));
		goalMap.Update();

		GD.Print("\nGoal Map After Move:");
		GD.Print(goalMap.ToString(5));

		// Note that, if you access positions in the goal map, they will be relative to the
		// viewport, not the original grid view.  For example, the following will print
		// "3", rather than "0":
		GD.Print($"\nGoalMap {Goal}: {goalMap[Goal]}");

		// To get the value of a global coordinate, we must perform the translation
		// ourself.
		GD.Print(
			$"Global {Goal}: {goalMap[Goal - viewport.ViewArea.Position]}");
	}

	public static void ExampleCode2()
	{
		// The map will have a width of 60 and height of 40
		var generator = new Generator(60, 40);

		// Add the steps to generate a map using the DungeonMazeMap built-in algorithm,
		// and generate the map.
		generator.ConfigAndGenerateSafe(gen =>
		{
			gen.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps());
		});
		var wallFloorValues = generator.Context.GetFirst<ISettableGridView<bool>>("WallFloor");
			
		GD.Print(wallFloorValues.ExtendToString(elementStringifier: b => b ? "." : "#"));
	}
}