using System;

public class NPCBuildingAction : INPCAction
{
    public int Value {get; set;}
    public Faction Faction {get; private set;}
    public Building Building {get; private set;}
    public Tile Tile {get; private set;}

    public NPCBuildingAction(Faction faction, Building building, Tile tile)
    {
        Faction = faction;
        Building = building;
        Tile = tile;

    }
    public void ExecuteAction()
    {
        PlaceBuilingOnTile();
    }

    private void PlaceBuilingOnTile()
    {
        if(Faction.SubtractResources(Building.BuildCost))
        {
            Tile.PlaceBuilding(Building);
            Tile.OccupyTile(Faction);
        }
    }
}