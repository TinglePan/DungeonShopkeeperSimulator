namespace DSS.Game.DuckTyping.Comps;

public class Tagged: BaseComp
{
    public string Tag;

    public static void Setup(DuckObject obj, string tag)
    {
        var taggedComp = obj.GetCompOrNew<Tagged>();
        taggedComp.Tag = tag;
    }
    
    public static string GetTag(DuckObject obj)
    {
        var taggedComp = obj.GetComp<Tagged>();
        return taggedComp.Tag;
    }
}