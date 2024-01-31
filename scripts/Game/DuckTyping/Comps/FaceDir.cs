using System;
using DSS.Common;

namespace DSS.Game.DuckTyping.Comps;

public class FaceDir: BaseComp
{
    public Watched<int> Dir = new (default);

    public static void Setup(Game game, Entity obj, Enums.Direction8 dir)
    {
        var faceDirComp = obj.GetCompOrNew<FaceDir>();
        faceDirComp.GameRef = game;
        faceDirComp.EntityRef = obj;
        faceDirComp.Dir.Value = (int)dir;
    }

    public bool IsBackStab(Enums.Direction8 fromDir)
    {
        return Dir.Value == (int)fromDir;
    }

    public bool IsSideStab(Enums.Direction8 fromDir)
    {
        var identicalBits = Utils.CountIdenticalBits(Dir.Value, (int)fromDir, 4);
        return identicalBits is > 1 and < 4;
    }

    public bool IsSideOrBackStab(Enums.Direction8 fromDir)
    {
        return IsBackStab(fromDir) || IsSideStab(fromDir);
    }
}