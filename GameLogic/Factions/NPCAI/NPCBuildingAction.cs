public class NPCBuildingAction : INPCAction
{
    public double Value {get; set;}
    public Faction Faction {get; private set;}
    public Building Building {get; private set;}
    public Tile Tile {get; private set;}
    private TileMapManager _tileMapManager;

    public NPCBuildingAction(Faction faction, Building building, Tile tile, TileMapManager tileMapManager)
    {
        Faction = faction;
        Building = building;
        Tile = tile;
        _tileMapManager = tileMapManager;

    }
    public void ExecuteAction()
    {
        _tileMapManager.BuildOnTile(Tile,Building,Faction);
    }
}