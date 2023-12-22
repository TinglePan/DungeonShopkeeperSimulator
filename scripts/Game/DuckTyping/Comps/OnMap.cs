using System;
using DSS.Common;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class OnMap: BaseComp
{
    public Guid MapId;
    public Vector2I Coord;
}