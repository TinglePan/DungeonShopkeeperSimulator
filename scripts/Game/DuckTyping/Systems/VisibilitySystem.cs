using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DSS.Common;
using DSS.Game.DuckTyping.Comps;
using Godot;

namespace DSS.Game.DuckTyping.Systems;

public class VisibilitySystem
{
    public static void UpdateVisibilityOf(DuckObject entity)
    {
        var viewshed = entity.GetComp<Viewshed>();
        var map = OnMap.GetMap(entity);
        var coord = OnMap.GetCoord(entity);
        IEnumerable<Vector2I> visibleCoords;
        if (entity.GetComp<FaceDir>(ensure: false) is { } faceDirComp)
        {
            var faceDir = faceDirComp.Dir;
            visibleCoords = map.GetVisibleCoords(coord, faceDir, viewshed.Angle, viewshed.Range);
        }
        else
        {
            visibleCoords = map.GetVisibleCoords(coord, Enums.Direction8.Up,
                360, viewshed.Range);
        }
        Viewshed.UpdateVisibility(entity, visibleCoords);
    }
    
    public static bool IsPlayerVisible(DuckObject entity)
    {
        if (entity.GetComp<Renderable>(ensure: false) is { HideWhenNotVisible:true })
        {
            var map = OnMap.GetMap(entity);
            var coord = OnMap.GetCoord(entity);
            if (map.PlayerTileVisibilities[map.CoordToIndex(coord)] != (byte)Enums.TileVisibility.Visible)
            {
                return false;
            }
        }
        return true;

    }

}