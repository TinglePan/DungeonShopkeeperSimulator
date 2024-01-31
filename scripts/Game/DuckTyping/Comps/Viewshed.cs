using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DSS.Common;
using Godot;
using GoRogue.FOV;

namespace DSS.Game.DuckTyping.Comps;

public class Viewshed: BaseComp
{
    public int Range;
    public int Angle;
    public bool IsXRay;
    
    [JsonIgnore]
    public HashSet<Vector2I> Region;

    public Action<HashSet<Vector2I>, HashSet<Vector2I>> OnRegionChanged;

    public static void Setup(Game game, Entity obj, int range, int angle, bool isXRay=false)
    {
        var viewshedComp = obj.GetCompOrNew<Viewshed>();
        viewshedComp.GameRef = game;
        viewshedComp.EntityRef = obj;
        viewshedComp.Range = range;
        viewshedComp.Angle = angle;
        viewshedComp.IsXRay = isXRay;
    }
    
    public void UpdateRegion()
    {
        var viewshed = EntityRef.GetComp<Viewshed>();
        var onMapComp = EntityRef.GetComp<OnMap>();
        var map = onMapComp.Map;
        var coord = onMapComp.Coord.Value;
        IEnumerable<Vector2I> visibleCoords;
        if (EntityRef.GetComp<FaceDir>(ensure: false) is { } faceDirComp)
        {
            var faceDir = (Enums.Direction8)faceDirComp.Dir.Value;
            visibleCoords = map.GetVisibleTiles(coord, faceDir, viewshed.Angle, viewshed.Range);
        }
        else
        {
            visibleCoords = map.GetVisibleTiles(coord, Enums.Direction8.Up,
                360, viewshed.Range);
        }
        var oldRegion = viewshed.Region;
        oldRegion ??= Enumerable.Empty<Vector2I>().ToHashSet();
        viewshed.Region = visibleCoords.ToHashSet();
        OnRegionChanged?.Invoke(oldRegion, Region);
    }

    public bool ProvidePlayerVision()
    {
        Profile.StartWatch("ProvidePlayerVision");
        if (EntityRef.GetComp<Faction>(ensure: false) is {} factionComp)
        {
            if (factionComp.FactionId == Enums.FactionId.Player)
            {
                return true;
            }
        }
        Profile.EndWatch("ProvidePlayerVision");
        Profile.PrintResultsEvery("ProvidePlayerVision", 10);
        return false;
    }
}