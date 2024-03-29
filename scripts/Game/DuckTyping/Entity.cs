﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.DuckTyping;

public class Entity: IEquatable<Entity>
{
    public Guid Id = Guid.NewGuid();
    private Dictionary<Type, Dictionary<string, BaseComp>> _comps = new();

    public void AddComp(BaseComp comp, string tag="")
    {
        var type = comp.GetType();
        tag ??= "";
        _comps.TryAdd(type, new Dictionary<string, BaseComp>());
        _comps[type].TryAdd(tag, comp);
    }
    
    public void RemoveComp(BaseComp comp)
    {
        var type = comp.GetType();
        if (_comps.ContainsKey(type)) 
        {
            var kvp = _comps[type].FirstOrDefault(kvp => kvp.Value.Equals(comp));
            _comps[type].Remove(kvp.Key);
        }
    }
    
    public T GetComp<T>(string tag="", bool ensure=true) where T: BaseComp
    {
        var type = typeof(T);
        var res = GetComp((type, tag)) as T;
        if (ensure && res == null)
        {
            throw new Exception($"Component {type} not found on {this}");
        }

        return res;
    }
    
    public T GetCompOrNew<T>(string tag="") where T: BaseComp, new()
    {
        var type = typeof(T);
        var res = GetComp((type, tag));
        if (res != null) return res as T;
        res = new T();
        AddComp(res, tag);
        return res as T;
    }
    
    public bool HasComp<T>(string tag="", bool precise=false) where T: BaseComp
    {
        var type = typeof(T);
        if (precise)
        {
            return _comps.ContainsKey(type) && _comps[type].ContainsKey(tag);
        }
        return GetComp((type, tag)) != null;
    }

    private BaseComp GetComp((Type, string) typeTagPair)
    {
        var (type, tag) = typeTagPair;
        tag ??= "";
        if (_comps.ContainsKey(type) && _comps[type].TryGetValue(tag, out var res))
        {
            return res;
        }
        foreach (var (testType, comps) in _comps)
        {
            if (type.IsAssignableFrom(testType))
            {
                return comps[tag];
            }
        }
        foreach (var testType in _comps.Keys)
        {
            if (!type.IsAssignableFrom(testType)) continue;
            if (_comps[testType].TryGetValue(tag, out res))
            {
                return res;
            }
        }

        return null;
    }

    public bool CheckComps(IEnumerable<(Type, string)> typeTagPairs)
    {
        foreach (var pair in typeTagPairs)
        {
            if (GetComp(pair) == null) return false;
        }
        return true;
    }

    public bool Equals(Entity other)
    {
        return Id.Equals(other?.Id);
    }
}