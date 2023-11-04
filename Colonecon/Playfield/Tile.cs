using System.Numerics;

public class Tile
{
    public TileType Type { get; private set; }

    public Tile(TileType type)
    {
        Type = type;
    }
}

public enum TileType
{
    Water,
    Forest,
    Mountain,
    Plain
}
