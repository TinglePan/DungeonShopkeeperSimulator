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

	public enum EntityRenderOrder
	{
		Terrain = 0,
		Ground = 1,
		Wall = 2,
		Furniture = 3,
		Items = 4,
		Creature = 5,
		Overlay = 99,
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

	[Flags]
	public enum PathFindingFlag
	{
		None = 0,
		IsPlayer = 1,
		CanFly = 2,
		CanOpenDoor = 4,
		CanBreakObstacle = 8,
	}

	[Flags]
	public enum ViewFlag
	{
		None = 0,
		XRay = 1,
	}

	public enum TileType
	{
		Terrain,
		Wall,
		Furniture,
		Creature,
		Overlay
	}

	[Flags]
	public enum EntityFlag
	{
		Occlusion = 1,
		Dynamic = 2,
	}

	public enum CollisionLayer
	{
		Wall,
		Water,
		Furniture,
		FurnitureObstacle,
		HostileCreature,
		FriendlyCreature,
	}
}
