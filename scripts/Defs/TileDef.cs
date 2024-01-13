using DSS.Common;

namespace DSS.Defs;

public class TileDef: BaseDef
{
    public int Id;
    public string DisplayName;
    public string Description;
    public Enums.TileType Type;
    public Enums.TileFlag[] Flags;
}