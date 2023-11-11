using System.Numerics;
using System.Runtime.CompilerServices;

public class Tile
{
    public int MiraStartDeposit { get; private set; }
    public int MiraCurrentDeposit { get; private set; }
    public Faction TileOwner{ get; private set; }
    public Building Building{ get; private set; }
    public Tile(int miraDeposit)
    {
        MiraStartDeposit = miraDeposit;
        MiraCurrentDeposit = miraDeposit;
    }

    public void OccupyTile(Faction faction)
    {
        TileOwner = faction;
    }

    public void PlaceBuilding(Building building)
    {
        Building = building;
    }
}
