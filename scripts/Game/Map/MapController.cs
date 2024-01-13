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

    public void Init(Map map)
    {
        MapRef = map;
        map.OnTileChanged += UpdateTile;
        TileMapRef.TileSet.TileSize = Constants.TileSize;
        for (int i = 0; i < TileMapRef.GetLayersCount(); i++)
        {
            var layerName = TileMapRef.GetLayerName(i);
            if (layerName == "Ground")
            {
                TileMapRef.SetLayerZIndex(i, (int)Enums.ObjectRenderOrder.Ground);
            }
            else if (layerName == "Wall")
            {
                TileMapRef.SetLayerZIndex(i, (int)Enums.ObjectRenderOrder.Wall);
            }
        }
        var tileSetAtlasSource = TileMapRef.TileSet.GetSource(0) as TileSetAtlasSource;
        var texturePath = tileSetAtlasSource?.Texture.ResourcePath;
        if (texturePath != null)
        {
            var defPath = Utils.TexturePathToDefPath(texturePath);
            var defId = new DefId() { FilePath = defPath, InFileStrId = Path.GetFileNameWithoutExtension(defPath)};
            _tileAtlasDef = Game.Instance.DefStore.GetDef<TileAtlasDef>(defId);
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
            TileMapRef.SetCell(WallLayerId, coord, sourceId:WallLayerSourceId, _tileAtlasDef.TileIdToCoordMap[(int)wallTileId]);
        }
    }

    private void ClearCell(int layerId, Vector2I coord)
    {
        TileMapRef.SetCell(layerId, coord, sourceId:-1, new Vector2I(-1, -1));
    }
}