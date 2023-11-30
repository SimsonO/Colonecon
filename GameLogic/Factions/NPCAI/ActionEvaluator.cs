using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework.Audio;

public class ActionEvaluator
{
    private Random _rnd;
    public ActionEvaluator()
    {
        _rnd = new Random();
    }
    public void Evaluate(INPCAction action)
    {
        switch (action)
        {
            case NPCBuildingAction buildAction:
                EvaluateBuildAction(buildAction);
                break;
            case INPCTradingAction tradeAction:
                EvaluateTradeAction(tradeAction);
                break;
            case NPCResearchAction researchAction:
                EvaluateResearchAction(researchAction);
                break;
            default:
                throw new InvalidOperationException("Unknown action type");
        }
    }

    public void Evaluate(List<INPCAction> actions)
    {
        foreach(INPCAction action in actions)
        {
            Evaluate(action);
        }
    }

    private void EvaluateBuildAction(NPCBuildingAction action)
    {
        double value = 0;
        value = EvaluateProduce(action, value);
        value = EvaluateConsume(action, value);
        value -= action.Tile.MiraCurrentDeposit; // OpportunityCost
        if(action.Building.Name == "Science Laboratory" && !action.Faction.ResearchEnabled)
        {
            value += 1000;
        }
        action.Value = value;
    }

    private double EvaluateProduce(NPCBuildingAction action, double value)
    {
        foreach (ResourceType resource in action.Building.ProductionRates.Keys)
        {
            int consume = 0;
            if (action.Faction.ResourceConsume.ContainsKey(resource))
            {
                consume = action.Faction.ResourceConsume[resource];
            }
            int produce = 0;
            if (action.Faction.ResourceProduce.ContainsKey(resource))
            {
                produce = action.Faction.ResourceProduce[resource];
            }
            int stock = 0;
            if (action.Faction.ResourceStock.ContainsKey(resource))
            {
                stock = action.Faction.ResourceStock[resource];
            }
            if (resource == ResourceType.Mira)
            {
                value += 10 * action.Tile.MiraCurrentDeposit; //2* to mitigate Opportunity costs
            }
            else if(resource == ResourceType.Energy)
            {
                if (consume > produce)
                {
                    value += 1000;
                }
                else
                {
                    value += 50 / Math.Max((produce - consume),1) - stock;
                }
            }
            else if(resource == ResourceType.Communium)
            {
                value += Math.Max(1000 / Math.Max((produce - consume),1), 5);
            }
        }
        return value;
    }

    private double EvaluateConsume(NPCBuildingAction action, double value)
    {
        foreach (ResourceType resource in action.Building.ConsumptionRates.Keys)
        {
            int consume = 0;
            if (action.Faction.ResourceConsume.ContainsKey(resource))
            {
                consume = action.Faction.ResourceConsume[resource];
            }
            int produce = 0;
            if (action.Faction.ResourceProduce.ContainsKey(resource))
            {
                produce = action.Faction.ResourceProduce[resource];
            }
            int stock = 0;
            if (action.Faction.ResourceStock.ContainsKey(resource))
            {
                stock = action.Faction.ResourceStock[resource];
            }
            if (resource == ResourceType.Mira)
            {
                value -= 10 * action.Building.ProductionRates[resource] ;
            }
            else if (consume + action.Building.ConsumptionRates[resource] > produce + stock)
            {
                value -= 100 * action.Building.ConsumptionRates[resource];
            }
            else
            {
                value -= action.Building.ConsumptionRates[resource] * 10 * Math.Max(consume - produce, 1);
            }
        }
        return value;
    }

    private void EvaluateTradeAction(INPCTradingAction action)
    {
        if(action.Faction.ResourceStock[action.TradingPartner.FactionResource] < 5)
        {
            action.Value = 1000;
        }
        else if(action.Faction.ResourceStock[action.TradingPartner.FactionResource] < 15)
        {
            action.Value = _rnd.Next(10-action.Faction.ResourceStock[action.TradingPartner.FactionResource],10);// just do it sometimes
        }
        
    }

    private void EvaluateResearchAction(NPCResearchAction action)
    {
        switch (action.Upgrade.Name)
        {
            case "Mine Deeper": 
                EvaluateMineDeeperAction(action);
                break;
            case "Mine Faster":
                EvaluateMineFaster(action);
                break;
            case "Upgrade Factories":
                EvaluateUpgradeFactories(action);
                break;
            default:
                throw new InvalidOperationException("Unknown action type");
        }
    }

    private void EvaluateMineDeeperAction(NPCResearchAction action)
    {
        int value = EvaluateCost(action);
        foreach (Tile tile in action.Faction.Territory)
        {
            if(tile.Building?.ProductionRates?.ContainsKey(ResourceType.Mira) ?? false)
            {
                if(tile.MiraCurrentDeposit < 2 * (tile.Building.ProductionRates[ResourceType.Mira]))
                {
                    value += tile.Building.ProductionRates[ResourceType.Mira];
                }
                else
                {
                    value += Math.Min(tile.Building.ProductionRates[ResourceType.Mira], 2); //arbitrary value if enough mira there. Would be better to have turncounter here
                }
            }
        }
        action.Value = value;
    }

    private void EvaluateMineFaster(NPCResearchAction action)
    {
        int value = EvaluateCost(action);
        foreach (Tile tile in action.Faction.Territory)
        {
            if(tile.Building?.ProductionRates?.ContainsKey(ResourceType.Mira) ?? false)
            {
                value += tile.Building.ProductionRates[ResourceType.Mira];
            }
        }
        action.Value = value;
    }

    private void EvaluateUpgradeFactories(NPCResearchAction action)
    {
        int value = EvaluateCost(action);
        foreach (Tile tile in action.Faction.Territory)
        {
            if ((tile.Building?.Name ?? "null") == "Factory") // i hate the code  write today
            {
                value += 5;
            }
        }
    }

    private int EvaluateCost(NPCResearchAction action)
    {
        int value = 0;
        foreach (ResourceType resource in action.Upgrade.UpgradeCost.Keys)
        {
            value -= action.Upgrade.UpgradeCost[resource] * action.Faction.TradePrice; // i just assume that the Trade is the same for all factions
        }
        return value;
    }
}
