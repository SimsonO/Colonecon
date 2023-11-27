using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class Player : Faction
{
    public Player(string name, Color color, ResourceType factionResource) : base(name, color, factionResource)
    {
    }

    public bool IncreaseAvailableTradeAmount(int amount)
    {
        if(ResourceStock[FactionResource] >= amount)
        {
            AvailableTradeAmountFactionResource += amount;
            SubtractResources(FactionResource, amount);
            return true;
        }        
        return false;
    }

    public bool DecreaseAvailableTradeAmount(int amount)
    {
        if(AvailableTradeAmountFactionResource >= amount)
        {
            AvailableTradeAmountFactionResource -= amount;
            AddRessources(FactionResource,amount);
            return true;
        }
        return false;
    }
}