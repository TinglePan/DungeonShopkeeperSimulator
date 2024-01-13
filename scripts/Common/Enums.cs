using System;
namespace DSS.Common;

public static class Enums
{
	[Flags]
	public enum Direction4
	{
		Up = 1,
		North = Up,
		Right = 2,
		East = Right,
		Down = 4,
		South = Down,
		Left = 8,
		West = Left,
	}

	[Flags]
	public enum Direction5
	{
		Neutral = 0,
		Up = 1,
		North = Up,
		Right = 2,
		East = Right,
		Down = 4,
		South = Down,
		Left = 8,
		West = Left,
	}
	
	[Flags]
	public enum Direction8
	{
		Up = 1,
		North = Up,
		Right = 2,
		East = Right,
		Down = 4,
		South = Down,
		Left = 8,
		West = Left,
		UpLeft = 9,
		NorthWest = UpLeft,
		UpRight = 3,
		NorthEast = UpRight,
		DownLeft = 12,
		SouthWest = DownLeft,
		DownRight = 6,
		SouthEast = DownRight
	}

	[Flags]
	public enum Direction9
	{
		Neutral = 0,
		Up = 1,
		North = Up,
		Right = 2,
		East = Right,
		Down = 4,
		South = Down,
		Left = 8,
		West = Left,
		UpLeft = 9,
		NorthWest = UpLeft,
		UpRight = 3,
		NorthEast = UpRight,
		DownLeft = 12,
		SouthWest = DownLeft,
		DownRight = 6,
		SouthEast = DownRight
	}

	// public enum TileId
	// {
	// 	Floor,
	// 	Wall,
	// 	UpStairs,
	// 	Player,
	// 	Adventurer,
	// }

	[Flags]
	public enum TileFlag
	{
		Wall = 1,
		BlockLight = 2,
		ForbidItems = 4,
		HideItems = 8,
	}

	public enum TileType
	{
		Terrain,
		Entity,
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
	
	public enum DuckObjectTag
	{
		None,
		Building,
		Creature,
		Item,
	}

	public enum InputSource
	{
		None,
		LocalP1,
		RemoteP2,
		RemoteP3,
		RemoteP4,
	}

	public enum FactionId
	{
		Player,
		Adventurer,
		Monster,
	}

	public enum ActionCode
	{
		
	}
}
