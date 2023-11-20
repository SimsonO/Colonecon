using System;
using System.ComponentModel.DataAnnotations;

public class Tile
{
    public int MiraStartDeposit { get; private set; }
    public int MiraCurrentDeposit { get; private set; }
    public Faction TileOwner { get; private set; }
    public Building Building{ get; private set; }
    public Tile(int miraDeposit)
    {
        MiraStartDeposit = miraDeposit;
        MiraCurrentDeposit = miraDeposit;
    }

    public void OccupyTile(Faction faction)
    {
        faction.Territory.Add(this);
        TileOwner = faction;
    }

    public void PlaceBuilding(Building building)
    {
        Building = building;
    }

    public void DestroyBuilding()
    {
        Building = null;
    }

    public int MineMira(int produceRate)
    {
        int actualProduceRate = Math.Min(MiraCurrentDeposit, produceRate);
        MiraCurrentDeposit -=actualProduceRate;
        return actualProduceRate;
    }

}
