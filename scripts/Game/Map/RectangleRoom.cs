using System.Collections.Generic;
using System.Linq;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game;

public class RectangleRoom : Room
{
    public Vector2I TopLeft;
    public Vector2I Dimension;
    public Vector2I BottomRight => TopLeft + Dimension;

    public override IEnumerable<Vector2I> IterTiles()
    {
        var topLeft = TopLeft;
        var bottomRight = BottomRight;
        for (var x = topLeft.X; x < bottomRight.X; x++)
        {
            for (var y = topLeft.Y; y < bottomRight.Y; y++)
            {
                yield return new Vector2I(x, y);
            }
        }
    }

    public override bool IsCoordInRoom(Vector2I coord)
    {
        var topLeft = TopLeft;
        var bottomRight = BottomRight;
        return coord.X >= topLeft.X && coord.X < bottomRight.X && coord.Y >= topLeft.Y && coord.Y < bottomRight.Y;
    }

    public override Vector2I RandomCoordInRoom()
    {
        var tiles = IterTiles().ToList();
        var index = Game.Instance.GameState.Rand.Next(tiles.Count());
        return tiles.ElementAtOrDefault(index);
    }
}