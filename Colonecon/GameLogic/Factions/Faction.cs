using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

public abstract class Faction
{
    public string Name {get; private set;}
    public Color Color {get; private set;}
    public ResourceType FactionResource {get; private set;}
    public Dictionary<ResourceType, int> RessourceStock {get; private set;}
    public List<Tile> FactionTerritory;
    public delegate void ResourcesChangedEventHandler(Faction faction);
    public static event ResourcesChangedEventHandler OnResourcesChanged;

    protected Faction(string name, Color color, ResourceType factionResource)
    {
        Name = name;
        Color = color;
        FactionResource = factionResource;
        FactionTerritory = new List<Tile>();
        RessourceStock = new Dictionary<ResourceType, int>
        {
            {ResourceType.Mira, 0 },
            {ResourceType.Energy, 0},
            {ResourceType.Communium, 30},
            {ResourceType.TerraSteel, 0},
            {ResourceType.Vorixium, 0},
            {ResourceType.Zytha, 0}
        };
    }

    public void AddRessources(Dictionary<ResourceType, int> ressourceAmount)
    {
        foreach(ResourceType resource in ressourceAmount.Keys)
        {
            RessourceStock[resource] += ressourceAmount[resource];
            OnResourcesChanged?.Invoke(this);
        }
    }

    public bool SubtractResources(Dictionary<ResourceType, int> ressourceAmount)
    {
        foreach(ResourceType resource in ressourceAmount.Keys)
        {
            if(RessourceStock[resource] < ressourceAmount[resource])
            {
               return false;
            }
        }
        foreach(ResourceType resource in ressourceAmount.Keys)
        {
            RessourceStock[resource] -= ressourceAmount[resource];
            OnResourcesChanged?.Invoke(this);
        }        
        return true; 
    }

    public bool EnoughResources(Building building)
    {
        Dictionary<ResourceType, int> buildingCost = building.BuildCost;
        foreach(ResourceType resource in buildingCost.Keys)
        {
            if(RessourceStock[resource] < buildingCost[resource])
            {
               return false;
            }
        }
        return true;
    }

    public void ProduceResources()
    {
        foreach(Tile tile in FactionTerritory)
        {
            if(tile.Building is not null)
            { 
                AddRessources(tile.Building.ProductionRates);
            }
        }
    }

    public void ConsumeResources()
    {
        foreach(Tile tile in FactionTerritory)
        {
            if(tile.Building is not null)
            {
                if(!SubtractResources(tile.Building.ConsumptionRates)) //if faction has not enough resources to support building it gets destroyed
                {
                    tile.DestroyBuilding();
                }

            }
        }
    }
    public abstract void EndTurn();
}