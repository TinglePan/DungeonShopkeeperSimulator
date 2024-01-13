using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.DuckTyping;

public class DuckObject
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
    
    public T GetComp<T>(string tag="") where T: BaseComp
    {
        var type = typeof(T);
        return GetComp((type, tag)) as T;
    }
    
    public T GetCompOrNew<T>(string tag="") where T: BaseComp, new()
    {
        var type = typeof(T);
        var res =GetComp((type, tag));
        if (res == null)
        {
            res = new T();
            AddComp(res, tag);
        }
        return res as T;
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
            if (type.IsAssignableFrom(testType))
            {
                if (_comps[testType].TryGetValue(tag, out res))
                {
                    return res;
                }
            }
        }

        return null;
    }

    public bool CheckComps((Type, string)[] typeTagPairs)
    {
        foreach (var pair in typeTagPairs)
        {
            if (GetComp(pair) == null) return false;
        }

        return true;
    }
    
}