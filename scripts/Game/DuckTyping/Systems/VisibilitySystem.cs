using System.Collections;
using System.Collections.Generic;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.DuckTyping.Systems;

public class VisibilitySystem
{
    public IEnumerable<Vector2I> GetVisibleCoords(DuckObject entity)
    {
        var viewshed = entity.GetComp<Viewshed>();
        var faceDir = FaceDir.GetFaceDir(entity);
        var map = OnMap.GetMap(entity);
        var coord = OnMap.GetCoord(entity);
        var visibleCoords = map.GetVisibleCoords(coord, faceDir, viewshed.Angle, viewshed.Range);
        return visibleCoords;
    }
}