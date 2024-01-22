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
    [Export] public int VisibilityOverlayLayerId;
    [Export] public int VisibilityOverlayLayerSourceId;

    public Map MapRef;

    private TileAtlasDef[] _tileAtlasDefs;
    private DefStore _defStoreRef => Game.Instance.DefStore;

    public void Init(Map map)
    {
        MapRef = map;
        // map.OnTileChanged += UpdateTile;
        map.OnTileChangeWitnessed += UpdateTile;
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
            else if (layerName == "VisibilityOverlay")
            {
                TileMapRef.SetLayerZIndex(i, (int)Enums.ObjectRenderOrder.VisibilityOverlay);
            }
            else
            {
                GD.PrintErr($"Unknown layer name: {layerName}");
            }
        }
        var nTileAtlases = TileMapRef.TileSet.GetSourceCount();
        _tileAtlasDefs = new TileAtlasDef[nTileAtlases];
        for (int i = 0; i < nTileAtlases; i++)
        {
            var tileSetAtlasSource = TileMapRef.TileSet.GetSource(i) as TileSetAtlasSource;
            var texturePath = tileSetAtlasSource?.Texture.ResourcePath;
            if (texturePath != null)
            {
                var defPath = Utils.TexturePathToDefPath(texturePath);
                var defId = new DefId() { FilePath = defPath, InFileStrId = Path.GetFileNameWithoutExtension(defPath)};
                _tileAtlasDefs[i] = Game.Instance.DefStore.GetDef<TileAtlasDef>(defId);
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
        var playerTileVisibility = MapRef.PlayerTileVisibilities[index];
        var overlayTileId = playerTileVisibility switch
        {
            (byte)Enums.TileVisibility.Revealed => _defStoreRef.GetTileId("revealed"),
            (byte)Enums.TileVisibility.Hidden => _defStoreRef.GetTileId("hidden"),
            _ => 0
        };
        var wallTileId = MapRef.WallTiles[index];
        if (wallTileId == 0)
        {
            ClearCell(WallLayerId, coord);
        }
        else
        {
            TileMapRef.SetCell(WallLayerId, coord, sourceId:WallLayerSourceId, _tileAtlasDefs[WallLayerSourceId].TileIdToCoordMap[(int)wallTileId]);
        }
        if (overlayTileId == 0)
        {
            ClearCell(VisibilityOverlayLayerId, coord);
        }
        else
        {
            TileMapRef.SetCell(VisibilityOverlayLayerId, coord, sourceId:VisibilityOverlayLayerSourceId,
                _tileAtlasDefs[VisibilityOverlayLayerSourceId].TileIdToCoordMap[overlayTileId]);
        }
    }

    private void ClearCell(int layerId, Vector2I coord)
    {
        TileMapRef.SetCell(layerId, coord, sourceId:-1, new Vector2I(-1, -1));
    }
}