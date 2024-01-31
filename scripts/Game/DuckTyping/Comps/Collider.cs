using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DSS.Common;
using DSS.Game.Actions;
using Godot;

namespace DSS.Game.DuckTyping.Comps;

public class Collider : BaseComp
{
    public enum MaskType
    {
        Collision,
        Melee,
        SwapPos
    }
    
    public HashSet<Enums.CollisionLayer> Layers;
    public Dictionary<MaskType, HashSet<Enums.CollisionLayer>> Masks = new Dictionary<MaskType, HashSet<Enums.CollisionLayer>>();

    public static void Setup(Game game, Entity obj, IEnumerable<Enums.CollisionLayer> layers,
        IEnumerable<Enums.CollisionLayer> collisionMasks, IEnumerable<Enums.CollisionLayer> meleeMasks=null,
        IEnumerable<Enums.CollisionLayer> swapPosMasks=null)
    {
        var colliderComp = obj.GetCompOrNew<Collider>();
        colliderComp.GameRef = game;
        colliderComp.EntityRef = obj;
        colliderComp.Layers = layers.ToHashSet();
        foreach (var maskType in Enum.GetValues(typeof(MaskType)))
        {
            colliderComp.Masks[(MaskType)maskType] = new HashSet<Enums.CollisionLayer>();
        }
        foreach (var layer in collisionMasks)
        {
            colliderComp.Masks[MaskType.Collision].Add(layer);
        }
        foreach (var layer in meleeMasks ?? new [] { Enums.CollisionLayer.HostileCreature })
        {
            colliderComp.Masks[MaskType.Melee].Add(layer);
        }
        if (swapPosMasks != null)
        {
            
            foreach (var layer in swapPosMasks)
            {
                colliderComp.Masks[MaskType.Collision].Add(layer);
            }
        }
    }
    
    public void AddLayer(Enums.CollisionLayer layer)
    {
        Layers.Add(layer);
    }
    
    public void RemoveLayer(Enums.CollisionLayer layer)
    {
        Layers.Remove(layer);
    }
    
    public void AddMask(MaskType maskType, Enums.CollisionLayer layer)
    {
        Masks[maskType].Add(layer);
    }
    
    public void RemoveMask(MaskType maskType, Enums.CollisionLayer layer)
    {
        Masks[maskType].Remove(layer);
    }
    
    public bool HasLayer(Enums.CollisionLayer layer)
    {
        return Layers.Contains(layer);
    }

    public bool InteractWith(MaskType maskType, Collider other)
    {
        foreach (var layer in Masks[maskType])
        {
            if (other.Layers.Contains(layer))
            {
                return true;
            }
        }
        return false;
    }

    public bool CanPass(Collider other)
    {
        return !InteractWith(MaskType.Collision, other) && !InteractWith(MaskType.Melee, other);
    }

}