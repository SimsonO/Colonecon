using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Player : Faction
{
    public Player(string name, Color color, ResourceType factionResource) : base(name, color, factionResource)
    {
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
    public override void EndTurn()
    {
        ProduceResources();
        ConsumeResources();
    }
}