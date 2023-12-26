using System;
namespace DSS.Common;

public static class Enums
{
	[Flags]
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

	[Flags]
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
	
	[Flags]
	public enum Direction8
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

	[Flags]
	public enum Direction9
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

	public enum TileId
	{
		Floor = 0,
		Wall = 1,
		Player = 2,
	}

	public enum ObjectRenderOrder
	{
		Terrain = 0,
		Ground = 1,
		Wall = 2,
		Building = 3,
		Items = 4,
		Creature = 5,
	}
}
