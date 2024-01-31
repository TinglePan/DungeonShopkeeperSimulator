using System;
using DSS.Common;
using DSS.Defs;
using DSS.Game.DuckTyping;
using DSS.Game.DuckTyping.Comps;
using Godot;
using GoRogue.Pathing;
using ImpromptuInterface;
using Path = System.IO.Path;

namespace DSS.Game;

public partial class CreatureController: Node2D
{
    [Export] public Sprite2D SpriteRef;
    [Export] public PackedScene FaceArrowPrefab;
    private Entity _creature;
    private Node2D _faceArrowNodeRef;

    public void Init(Entity creature)
    {
        _creature = creature;
        var onMapComp = creature.GetComp<OnMap>();
        onMapComp.Coord.OnChanged += (fromCoord, toCoord) =>
        {
            var tween = GetTree().CreateTween();
            Vector2 toPos = toCoord * Constants.TileSize + SpriteRef.Texture.GetSize() / 2;
            tween.TweenProperty(this, "position", toPos, 0.1f);
            // Position = toCoord * Constants.TileSize;
            var renderableComp = creature.GetComp<Renderable>();
            renderableComp.UpdateVisibility();
        };
        var renderableComp = creature.GetComp<Renderable>();
        renderableComp.IsVisible.OnChanged += (_, value) =>
        {
            SpriteRef.Visible = value;
        };
        var renderableType = renderableComp.GetType();
        if (renderableType == typeof(TileRenderable))
        {
            AtlasTexture texture = new AtlasTexture();
            var texturePath = renderableComp.AtlasPath;
            texture.Atlas = GD.Load<Texture2D>(texturePath);
            var defPath = Utils.TexturePathToDefPath(texturePath);
            var defId = new DefId() { FilePath = defPath, InFileStrId = Path.GetFileNameWithoutExtension(defPath) };
            var def = Game.Instance.DefStore.GetDef<TileAtlasDef>(defId);
            var tileCoord = ((TileRenderable)renderableComp).Offset;
            texture.Region = new Rect2(tileCoord.X * def.TileSize.X, tileCoord.Y * def.TileSize.Y,
                def.TileSize.X, def.TileSize.Y);
            SpriteRef.Texture = texture;
        } else if (renderableType == typeof(SpriteRenderable))
        {
            var texturePath = renderableComp.AtlasPath;;
            Texture2D texture = GD.Load<Texture2D>(texturePath);
            SpriteRef.Texture = texture;
        }
        else
        {
            GD.PrintErr("Unknown renderable type");
        }
        var coord = onMapComp.Coord.Value;
        Position = coord * Constants.TileSize + SpriteRef.Texture.GetSize() / 2;
        // SpriteRef.Offset = SpriteRef.Texture.GetSize() / 2;
        SpriteRef.ZIndex = (int)Enums.EntityRenderOrder.Creature;
        
        if (creature.GetComp<FaceDir>(ensure:false) is {} faceDirComp)
        {
            _faceArrowNodeRef = FaceArrowPrefab.Instantiate<Node2D>();
            _faceArrowNodeRef.Name = "FaceArrow";
            var faceDir = faceDirComp.Dir;
            _faceArrowNodeRef.Rotation = Utils.DirToAngle((Enums.Direction8)faceDir.Value);
            faceDir.OnChanged += (fromDir, toDir) =>
            {
                var tween = GetTree().CreateTween();
                var toDeg = Mathf.DegToRad(Utils.DirToAngle((Enums.Direction8)toDir));
                tween.TweenProperty(_faceArrowNodeRef, "rotation", toDeg, 0.1f);
            };
            AddChild(_faceArrowNodeRef);
        }
    }
}