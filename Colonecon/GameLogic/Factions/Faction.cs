using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

public abstract class Faction
{
    public string Name {get; private set;}
    public Color Color {get; private set;}
    public Dictionary<ResourceType, int> RessourceStock {get; private set;}

    protected Faction(string name, Color color)
    {
        Name = name;
        Color = color;
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
        }
    }

    public bool SubtractRessources(Dictionary<ResourceType, int> ressourceAmount)
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
}