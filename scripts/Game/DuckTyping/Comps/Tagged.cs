namespace DSS.Game.DuckTyping.Comps;

public class Tagged: BaseComp
{
    public string Tag;

    public static void Setup(Game game, Entity obj, string tag)
    {
        var taggedComp = obj.GetCompOrNew<Tagged>();
        taggedComp.GameRef = game;
        taggedComp.EntityRef = obj;
        taggedComp.Tag = tag;
    }
}