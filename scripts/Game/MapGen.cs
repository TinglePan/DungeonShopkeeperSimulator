using System;
using GoRogue.MapGeneration;

namespace DSS.Game;

public class MapGen
{
	public static void Test()
	{
		var generator = new Generator(60, 40);

		// Add the steps to generate a map using the DungeonMazeMap built-in algorithm,
		// and generate the map.
		generator.ConfigAndGenerateSafe(gen =>
		{
			gen.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps());
		});
	}
}
