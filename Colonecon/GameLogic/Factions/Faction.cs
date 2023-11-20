using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

public abstract class Faction
{
    public string Name {get; private set;}
    public Color Color {get; private set;}
    public ResourceType FactionResource {get; private set;}
    public Dictionary<ResourceType, int> RessourceStock {get; private set;}
    public Dictionary<ResourceType, int> ResourceConsume {get; private set;}
    public Dictionary<ResourceType, int> ResourceProduce {get; private set;}
    public List<Tile> Territory;
    public delegate void ResourcesChangedEventHandler(Faction faction);
    public static event ResourcesChangedEventHandler OnResourcesChanged;

    protected Faction(string name, Color color, ResourceType factionResource)
    {
        Name = name;
        Color = color;
        FactionResource = factionResource;
        Territory = new List<Tile>();
        RessourceStock = new Dictionary<ResourceType, int>
        {
            {ResourceType.Mira, 0 },
            {ResourceType.Energy, 0},
            {ResourceType.Communium, 30},
            {ResourceType.TerraSteel, 0},
            {ResourceType.Vorixium, 0},
            {ResourceType.Zytha, 0}
        };
        ResourceConsume = new Dictionary<ResourceType, int>();
        ResourceProduce = new Dictionary<ResourceType, int>();

        TileMapManager.OnBuildingPlaced += UpdateProduce;
        TileMapManager.OnBuildingPlaced += UpdateConsume;
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
        foreach(Tile tile in Territory)
        {
            if(tile.Building is not null)
            { 
                if(tile.Building.ProductionRates is not null)
                {
                    Dictionary<ResourceType, int> producedResources = tile.Building.ProductionRates;
                    if(producedResources.ContainsKey(ResourceType.Mira))
                    {
                       producedResources[ResourceType.Mira] = tile.MineMira(producedResources[ResourceType.Mira]);
                    }
                    AddRessources(producedResources);
                }                
            }
        }
    }

    public void ConsumeResources()
    {
        foreach(Tile tile in Territory)
        {
            if(tile.Building is not null)
            {
                if(tile.Building.ConsumptionRates is not null)
                {
                    if(!SubtractResources(tile.Building.ConsumptionRates)) //if faction has not enough resources to support building it gets destroyed
                    {
                        tile.DestroyBuilding();
                    }
                }          
            }
        }
    }

    private void UpdateConsume()
    {
        foreach(Tile tile in Territory)
        {
            if(tile.Building is not null)
            {
                if(tile.Building.ConsumptionRates is not null)
                {
                    foreach(ResourceType resource in tile.Building.ConsumptionRates.Keys)
                    {
                        if(ResourceConsume.ContainsKey(resource))
                        {
                            ResourceConsume[resource] += tile.Building.ConsumptionRates[resource];
                        } 
                        else
                        {
                            ResourceConsume.Add(resource, tile.Building.ConsumptionRates[resource]);
                        }
                    }
                }          
            }
        }
    }
    private void UpdateProduce()
    {
        foreach(Tile tile in Territory)
        {
            if(tile.Building is not null)
            {
                if(tile.Building.ProductionRates is not null)
                {
                    foreach(ResourceType resource in tile.Building.ProductionRates.Keys)
                    {
                        if(ResourceProduce.ContainsKey(resource))
                        {
                            ResourceProduce[resource] += tile.Building.ProductionRates[resource];
                        } 
                        else
                        {
                            ResourceProduce.Add(resource, tile.Building.ProductionRates[resource]);
                        }
                    }
                }          
            }
        }
    }
    public abstract void EndTurn();
}