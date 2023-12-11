namespace DSS.Game.Components;

public record Named
{
    public string Name;
    public Named(string name)
    {
        Name = name;
    }
}