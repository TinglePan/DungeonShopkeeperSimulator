using System;
using System.Collections.Generic;
using System.Linq;
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
            return Comps.TryGetValue(type, out var comp) ? comp as T : null;
        }
        else
        {
            if (!TaggedComps.ContainsKey(type))
            {
                return null;
            }
            return TaggedComps[type].TryGetValue(tag, out var comp) ? comp as T : null;
        }
    }
    
}