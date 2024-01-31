using DSS.Common;

namespace DSS.Defs;

public class TileDef: BaseDef
{
    public int Id;
    public Enums.TileType Type;
    public string DisplayName;
    public string Description;
    public Enums.EntityFlag[] Flags;
}