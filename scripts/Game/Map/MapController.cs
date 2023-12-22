using System.IO;
using DSS.Common;
using DSS.Defs;
using Godot;

namespace DSS.Game;

public partial class MapController : Node2D
{
    [Export]
    public TileMap TileMapRef;

    [Export] public int GroundLayerId;
    [Export] public int WallLayerId;
    [Export] public int GroundLayerSourceId;
    [Export] public int WallLayerSourceId;

    public Map MapRef;

    private TileAtlasDef _tileAtlasDef;

    private static string TexturePathToDefPath(string texturePath)
    {
        return $"res://defs/{Path.GetFileNameWithoutExtension(texturePath.GetBaseName())}.json";
    }
    
    public void Init(Map map)
    {
        MapRef = map;
        map.OnTileChanged += UpdateTile;
        DefStore.Instance.Defs.TryGetValue(typeof(TileAtlasDef), out var tileAtlasDefs);
        if (tileAtlasDefs != null)
        {
            var tileSetAtlasSource = TileMapRef.TileSet.GetSource(0) as TileSetAtlasSource;
            var texturePath = tileSetAtlasSource?.Texture.ResourcePath;
            if (texturePath != null)
            {
                _tileAtlasDef = DefStore.Instance.GetDef<TileAtlasDef>(TexturePathToDefPath(texturePath));
            }
        }
        UpdateTiles();
    }

    public void UpdateTiles()
    {
        for (var i = 0; i < MapRef.Size; i++)
        {
            var coord = MapRef.IndexToCoord(i);
            UpdateTile(coord);
        }
    }

    public void UpdateTile(Vector2I coord)
    {
        var index = MapRef.CoordToIndex(coord);
        var wallTileId = MapRef.WallTiles[index];
        if (wallTileId == 0)
        {
            ClearCell(WallLayerId, coord);
        }
        else
        {
            TileMapRef.SetCell(WallLayerId, coord, sourceId:WallLayerSourceId, _tileAtlasDef.TileIdToCoordMap[(Enums.TileId)wallTileId]);
        }
    }

    private void ClearCell(int layerId, Vector2I coord)
    {
        TileMapRef.SetCell(layerId, coord, sourceId:-1, new Vector2I(-1, -1));
    }
}