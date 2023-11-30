using System;
namespace DSS.Ecs.Components;

public class Coord : ComponentBase
{
	public int X;
	public int Y;

	public Coord(int x, int y)
	{
		X = x;
		Y = y;
	}

}