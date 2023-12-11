using System;
namespace DSS.Game.Components;

public record Coord
{
	public int X;
	public int Y;

	public Coord(int x, int y)
	{
		X = x;
		Y = y;
	}

}