using System.Collections.Generic;
using Microsoft.Xna.Framework;

public abstract class Faction
{
    public string Name {get; private set;}
    public Color Color {get; private set;}
    public ResourceType FactionResource {get; private set;}
    public int AvailableTradeAmountFactionResource;
    public int TradePrice;
    public int FactionResourcePrice {get; private set;}
    public Dictionary<ResourceType, int> ResourceStock {get; private set;}
    public Dictionary<ResourceType, int> ResourceConsume {get; private set;}
    public Dictionary<ResourceType, int> ResourceProduce {get; private set;}
    public List<Tile> Territory;
    public ResearchUpgrade MineDeeperUpgrade;
    public ResearchUpgrade MineFasterUpgrade;
    public ResearchUpgrade FactoryMineUpgrade;
    public List<ResearchUpgrade> ResearchUpgrades;

    public delegate void ResourcesChangedEventHandler(Faction faction);
    public static event ResourcesChangedEventHandler OnResourcesChanged;
    

    protected Faction(string name, Color color, ResourceType factionResource)
    {
        Name = name;
        Color = color;
        FactionResource = factionResource;
        FactionResourcePrice = 5;
        Territory = new List<Tile>();
        ResourceStock = new Dictionary<ResourceType, int>
        {
            {ResourceType.Mira, 0 },
            {ResourceType.Energy, 0},
            {ResourceType.Communium, 30},
            {ResourceType.TerraSteel, 5},
            {ResourceType.Vorixium, 5},
            {ResourceType.Zytha, 5}
        };
        ResourceConsume = new Dictionary<ResourceType, int>();
        ResourceProduce = new Dictionary<ResourceType, int>();
        MineDeeperUpgrade = new ResearchUpgrade()
        {
            Name = "Mine Deeper",
            Description = "Mira Mines won't reduce the deposit for the next turn",
            UpgradeCost = new Dictionary<ResourceType, int>
            {
                {ResourceType.Vorixium, 5},
                {ResourceType.Zytha, 5}
            }
        };
        MineFasterUpgrade = new ResearchUpgrade()
        {
            Name = "Mine Faster",
            Description = "Mira Mines will produce double the amount next turn",
            UpgradeCost = new Dictionary<ResourceType, int>
            {
                {ResourceType.TerraSteel, 5},
                {ResourceType.Zytha, 5}
            }
        };
        FactoryMineUpgrade = new ResearchUpgrade() 
        {
            Name = "Upgrad Factories",
            Description = "Factories will produce 5 Mira next turn",
            UpgradeCost =  new Dictionary<ResourceType, int>
            {
                {ResourceType.TerraSteel, 5},
                {ResourceType.Vorixium, 5}
            }
        };
        ResearchUpgrades = new List<ResearchUpgrade>
        {
            MineDeeperUpgrade,
            MineFasterUpgrade,
            FactoryMineUpgrade
        };

        TileMapManager.OnBuildingPlaced += UpdateProduce;
        TileMapManager.OnBuildingPlaced += UpdateConsume;
        Header.OnRestartGame += Reset;
    }

    public void Reset()
    {
        Territory = new List<Tile>();
        ResourceStock = new Dictionary<ResourceType, int>
        {
            {ResourceType.Mira, 0 },
            {ResourceType.Energy, 0},
            {ResourceType.Communium, 35},
            {ResourceType.TerraSteel, 5},
            {ResourceType.Vorixium, 5},
            {ResourceType.Zytha, 5}
        };
        ResourceConsume = new Dictionary<ResourceType, int>();
        ResourceProduce = new Dictionary<ResourceType, int>();
        MineDeeperUpgrade.Deactivate();
        MineFasterUpgrade.Deactivate();
        OnResourcesChanged?.Invoke(this);
    }

    public void AddRessources(Dictionary<ResourceType, int> ressourceAmount)
    {
        foreach(ResourceType resource in ressourceAmount.Keys)
        {
            AddRessources(resource, ressourceAmount[resource]);
        }
    }

    public void AddRessources(ResourceType resource, int amount)
    {
        ResourceStock[resource] += amount;
        OnResourcesChanged?.Invoke(this);
    }

    public bool SubtractResources(Dictionary<ResourceType, int> ressourceAmount)
    {
        foreach(ResourceType resource in ressourceAmount.Keys)
        {
            if(ResourceStock[resource] < ressourceAmount[resource])
            {
               return false;
            }
        }
        foreach(ResourceType resource in ressourceAmount.Keys)
        {
            ResourceStock[resource] -= ressourceAmount[resource];
            OnResourcesChanged?.Invoke(this);
        }        
        return true; 
    }

    public bool SubtractResources(ResourceType resource, int amount)
    {
        if(ResourceStock[resource] < amount)
        {
            return false;
        }
        ResourceStock[resource] -= amount;
        OnResourcesChanged?.Invoke(this);
        return true;
    }

    public bool EnoughResources(Building building)
    {
        Dictionary<ResourceType, int> buildingCost = building.BuildCost;
        foreach(ResourceType resource in buildingCost.Keys)
        {
            if(ResourceStock[resource] < buildingCost[resource])
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
                    if(FactoryMineUpgrade.Active && tile.Building.Name == "Factory")
                    {
                        producedResources.Add(ResourceType.Mira,5);
                    }
                    if(producedResources.ContainsKey(ResourceType.Mira))
                    {
                       if(MineFasterUpgrade.Active)
                       {
                            producedResources[ResourceType.Mira] *=2;
                       }
                       if(!MineDeeperUpgrade.Active)
                       {
                            producedResources[ResourceType.Mira] = tile.MineMira(producedResources[ResourceType.Mira]);
                       }                       
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

    private void UpdateConsume(Faction faction)
    {
        if(faction == this)
        {
            ResourceConsume.Clear();
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
            OnResourcesChanged?.Invoke(this);
        }
        
    }
    private void UpdateProduce(Faction faction)
    {
        if(faction == this)
        {
            ResourceProduce.Clear(); //alternativley could only add produce of new buildin but with this i also account for other changes
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
            OnResourcesChanged?.Invoke(this);
        }
        
    }

    public bool TradeFromHome(int amount)
    {   
        if(SubtractResources(ResourceType.Mira, amount * FactionResourcePrice))
        {
            AddRessources(FactionResource, amount);
            return true;
        }
        return false;
    }

    public void SellFactionResource(int amount)
    {
       Dictionary<ResourceType,int> price = new Dictionary<ResourceType, int>
       {
           { ResourceType.Mira, amount * TradePrice}
       };
       Dictionary<ResourceType, int> ware = new Dictionary<ResourceType, int>
       {
        {FactionResource, amount}
       };
       AddRessources(price);
       SubtractResources(ware);       
    }

    public bool BuyResources(ResourceType resource, int amount, int unitTradePrice)
    {
        Dictionary<ResourceType,int> price = new Dictionary<ResourceType, int>
        {
            { ResourceType.Mira, amount * unitTradePrice }
        };
        Dictionary<ResourceType, int> ware = new Dictionary<ResourceType, int>
        {
            {resource, amount}
        };
        
        bool enoughMira = SubtractResources(price); 
        if (enoughMira)
        {
            AddRessources(ware);
        }
        return enoughMira;
        
    }

    public void ActivateUprade(ResearchUpgrade upgrade)
    {
        //add check for Price here
        upgrade.SetActive();
    }
    public virtual void EndTurn()
    {
        ProduceResources();
        ConsumeResources();
        MineDeeperUpgrade.Deactivate();
        MineFasterUpgrade.Deactivate();
        FactoryMineUpgrade.Deactivate();
    }
}