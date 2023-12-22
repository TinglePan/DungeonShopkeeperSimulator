using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

namespace DSS.Game;

public abstract class Room
{
    public abstract IEnumerable<Vector2I> IterTiles();

    public abstract bool IsCoordInRoom(Vector2I coord);

    public virtual Vector2I RandomCoordInRoom()
    {
        throw new NotImplementedException();
    }
}