using System;
namespace DSS.Common;

public static class Enums
{
	public enum Direction4
	{
		Up = 1,
		North = Up,
		Down = 2,
		South = Down,
		Left = 4,
		West = Left,
		Right = 8,
		East = Right,
	}

	public enum Direction5
	{
		Neutral = 0,
		Up = 1,
		North = Up,
		Down = 2,
		South = Down,
		Left = 4,
		West = Left,
		Right = 8,
		East = Right,
	}
	
	public enum Direction8
	{
		Neutral = 0,
		Up = 1,
		North = Up,
		Down = 2,
		South = Down,
		Left = 4,
		West = Left,
		Right = 8,
		East = Right,
		UpLeft = 5,
		NorthWest = UpLeft,
		UpRight = 9,
		NorthEast = UpRight,
		DownLeft = 6,
		SouthWest = DownLeft,
		DownRight = 10,
		SouthEast = DownRight
	}

	public enum Direction9
	{
		Up = 1,
		North = Up,
		Down = 2,
		South = Down,
		Left = 4,
		West = Left,
		Right = 8,
		East = Right,
		UpLeft = 5,
		NorthWest = UpLeft,
		UpRight = 9,
		NorthEast = UpRight,
		DownLeft = 6,
		SouthWest = DownLeft,
		DownRight = 10,
		SouthEast = DownRight
	}
}
