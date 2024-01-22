using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Viewshed: BaseComp
{
    public int Range;
    public int Angle;
    
    [JsonIgnore]
    public IEnumerable<Vector2I> VisibleCoords;

    public Action OnVisibleCoordsChanged;

    public static void Setup(DuckObject obj, int range, int angle)
    {
        var viewshedComp = obj.GetCompOrNew<Viewshed>();
        viewshedComp.Range = range;
        viewshedComp.Angle = angle;
    }
    
    public static void UpdateVisibility(DuckObject obj, IEnumerable<Vector2I> visibleCoords)
    {
        var viewshedComp = obj.GetComp<Viewshed>();
        viewshedComp.VisibleCoords = visibleCoords;
        viewshedComp.OnVisibleCoordsChanged?.Invoke();
    }

    public static void WatchVisibilityChange(DuckObject obj, Action action)
    {
        var viewshedComp = obj.GetComp<Viewshed>();
        viewshedComp.OnVisibleCoordsChanged += action;
    }
    
    public static IEnumerable<Vector2I> GetVisibleCoords(DuckObject obj)
    {
        var viewshedComp = obj.GetComp<Viewshed>();
        return viewshedComp.VisibleCoords;
    }
}

// Viewshed that provides player visibility
public class PlayerViewshed : Viewshed
{
    
    public new static void Setup(DuckObject obj, int range, int angle)
    {
        var viewshedComp = obj.GetCompOrNew<PlayerViewshed>();
        Viewshed.Setup(obj, range, angle);
    }
}