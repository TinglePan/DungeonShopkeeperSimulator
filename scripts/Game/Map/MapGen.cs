using DSS.Common;
using Godot;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace DSS.Game;

public class MapGen
{
	public void DungeonMaze(Map map)
	{
		var dimension = map.Dimension;
		var generator = new Generator(dimension.X, dimension.Y);

		// Add the steps to generate a map using the DungeonMazeMap built-in algorithm,
		// and generate the map.
		generator.ConfigAndGenerateSafe(gen =>
		{
			gen.AddSteps(DefaultAlgorithms.DungeonMazeMapSteps());
		});
		
		var wallFloorValues = generator.Context.GetFirst<ISettableGridView<bool>>("WallFloor");
		var rooms = generator.Context.GetFirst<ItemList<Rectangle>>("Rooms");
		foreach (var pos in wallFloorValues.Positions())
		{
			if (wallFloorValues[pos])
			{
				map.WallTiles[pos.ToIndex(dimension.X)] = 0;
			}
			else
			{
				map.WallTiles[pos.ToIndex(dimension.X)] = (byte)Enums.TileId.Wall;
			}
		}

		foreach (var roomStep in rooms)
		{
			var topLeft = new Vector2I(roomStep.Item.X, roomStep.Item.Y);
			var roomDimension = new Vector2I(roomStep.Item.Width, roomStep.Item.Height);
			map.Rooms.Add(new RectangleRoom()
			{
				TopLeft = topLeft,
				Dimension = roomDimension
			});
		}
	}
}
