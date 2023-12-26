using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.DuckTyping;

public class DuckObject
{
    public Guid Id = Guid.NewGuid();
    protected Dictionary<Type, BaseComp> Comps = new();
    protected Dictionary<Type, Dictionary<string, BaseComp>> TaggedComps = new();
    
    public void AddComp(BaseComp comp, string tag=null)
    {
        var type = comp.GetType();
        if (tag == null)
        {
            Comps.TryAdd(type, comp);
        }
        else
        {
            if (!TaggedComps.ContainsKey(type))
            {
                TaggedComps[type] = new Dictionary<string, BaseComp>();
            }
            TaggedComps[type].Add(tag, comp);
        }
    }
    
    public void RemoveComp(BaseComp comp)
    {
        var type = comp.GetType();
        if (TaggedComps.ContainsKey(type))
        {
            var kvp = TaggedComps[type].FirstOrDefault(kvp => kvp.Value.Equals(comp));
            TaggedComps[type].Remove(kvp.Key);
        }
        else
        {
            Comps.Remove(type);
        }
    }
    
    public T GetComp<T>(string tag=null) where T: BaseComp
    {
        var type = typeof(T);
        if (tag == null)
        {
            if (Comps.TryGetValue(type, out var comp))
            {
                return comp as T;
            }
            foreach (var testType in Comps.Keys)
            {
                if (type.IsAssignableFrom(testType))
                {
                    return Comps[testType] as T;
                }
            }
        }
        else
        {
            if (TaggedComps.ContainsKey(type))
            {
                if (TaggedComps[type].TryGetValue(tag, out var comp))
                {
                    return comp as T;
                }
            }
            
            foreach (var testType in TaggedComps.Keys)
            {
                if (type.IsAssignableFrom(testType))
                {
                    if (TaggedComps[testType].TryGetValue(tag, out var comp))
                    {
                        return comp as T;
                    }
                }
            }
        }

        return null;
    }
    
}