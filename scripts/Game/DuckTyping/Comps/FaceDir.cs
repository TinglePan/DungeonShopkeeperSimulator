using System;
using DSS.Common;

namespace DSS.Game.DuckTyping.Comps;

public class FaceDir: BaseComp
{
    public Enums.Direction8 Dir;
    
    public Action<Enums.Direction8, Enums.Direction8> OnDirChanged;

    public static void Setup(DuckObject obj, Enums.Direction8 dir)
    {
        var faceDirComp = obj.GetCompOrNew<FaceDir>();
        faceDirComp.Dir = dir;
    }
    
    public static void WatchDirChange(DuckObject obj, Action<Enums.Direction8, Enums.Direction8> onDirChanged)
    {
        var onMapComp = obj.GetComp<FaceDir>();
        onMapComp.OnDirChanged += onDirChanged;
    }
    
    public static Enums.Direction8 GetFaceDir(DuckObject obj)
    {
        var faceDirComp = obj.GetComp<FaceDir>();
        return faceDirComp.Dir;
    }
    
    public static void SetFaceDir(DuckObject obj, Enums.Direction8 dir)
    {
        var faceDirComp = obj.GetComp<FaceDir>();
        var fromDir = faceDirComp.Dir;
        faceDirComp.Dir = dir;
        faceDirComp.OnDirChanged?.Invoke(fromDir, dir);
    }

    public static bool IsBackStab(DuckObject obj, Enums.Direction8 fromDir)
    {
        var faceDir = GetFaceDir(obj);
        return faceDir == fromDir;
    }

    public static bool IsSideStab(DuckObject obj, Enums.Direction8 fromDir)
    {
        var faceDir = GetFaceDir(obj);
        var identicalBits = Utils.CountIdenticalBits((int)faceDir, (int)fromDir, 4);
        return identicalBits is > 1 and < 4;
    }

    public static bool IsSideOrBackStab(DuckObject obj, Enums.Direction8 fromDir)
    {
        return IsBackStab(obj, fromDir) || IsSideStab(obj, fromDir);
    }
}