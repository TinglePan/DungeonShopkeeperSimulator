using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Dimension: BaseComp
{
    public int Width;
    public int Height;
    
    public Vector2I Rect => new Vector2I(Width, Height);
}