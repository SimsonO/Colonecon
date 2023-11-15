using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Xna.Framework;

public class NPCFaction : Faction
{
    public int AvailableTradeAmountFactionResource {get; private set;}
    public int TradePrice{get; private set;}
    private int _tradeThreshhold; //Only Resources exceeding the threshhold are up for trade
    public NPCFaction(string name, Color color, ResourceType factionResource) : base(name, color, factionResource)
    {
    }

    private void CalculateTradeAmount()
    {
        CalculateTradeTheshhold();
        AvailableTradeAmountFactionResource = Math.Max(RessourceStock[FactionResource] - _tradeThreshhold,0);
    }
    private void CalculateTradeTheshhold()
    {   
        _tradeThreshhold = 10; //add some Logic here to account for more need in faction
    }

    private void CalculateTradePrice()
    {
        TradePrice = 10; // add some Log here to account supply/demand
    }

    public void SellFactionResource(int amount)
    {
       Dictionary<ResourceType,int> price = new Dictionary<ResourceType, int>
       {
           { ResourceType.Mira, amount * TradePrice }
       };
       Dictionary<ResourceType, int> ware = new Dictionary<ResourceType, int>
       {
        {FactionResource, amount}
       };
       AddRessources(price);
       SubtractResources(ware);       
    }

    public override void EndTurn()
    {
        ProduceResources();
        ConsumeResources();
        CalculateTradeAmount();
        CalculateTradePrice();
    }
}