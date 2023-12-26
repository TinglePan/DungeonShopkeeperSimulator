namespace DSS.Game.DuckTyping.Comps;

public class Tagged: BaseComp
{
    public string Tag;
    
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
    
    public static string GetTag(DuckObject obj)
    {
        var taggedComp = obj.GetComp<Tagged>();
        return taggedComp.Tag;
    }
}