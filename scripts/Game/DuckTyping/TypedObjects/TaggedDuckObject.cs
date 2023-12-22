using DSS.Game.DuckTyping.Comps;

namespace DSS.Game.DuckTyping.TypedObjects;

public static class TaggedDuckObject
{
    public static bool CheckDuckType(DuckObject obj)
    {
        return obj.GetComp<Tagged>() != null;
    }

    public static void Setup(DuckObject obj, string tag)
    {
        var taggedComp = new Tagged
        {
            Tag = tag
        };
        obj.AddComp(taggedComp);
    }
    
    public static string Tag(DuckObject obj)
    {
        var taggedComp = obj.GetComp<Tagged>();
        return taggedComp.Tag;
    }
}