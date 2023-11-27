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
                value += 2 * action.Tile.MiraCurrentDeposit; //2* to mitigate Opportunity costs
            }
            else if (consume > produce)
            {
                value += 1000;
            }
            else
            {
                value += action.Building.ProductionRates[resource] * 10  * (Math.Max(1,30-produce))- stock;
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
        action.Value = 10 + _rnd.Next(-10,100);
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
            if(tile.MiraCurrentDeposit < 2 * (tile.Building?.ProductionRates[ResourceType.Mira] ?? 0))
            {
                 value += tile.Building?.ProductionRates[ResourceType.Mira] ?? 0;
            }
            else
            {
                value += Math.Min(tile.Building?.ProductionRates[ResourceType.Mira] ?? 0, 2); //arbitrary value if enough mira there. Would be better to have turncounter here
            }
            
        }
        action.Value = value;
    }

    private void EvaluateMineFaster(NPCResearchAction action)
    {
        int value = EvaluateCost(action);
        foreach (Tile tile in action.Faction.Territory)
        {
            value += tile.Building?.ProductionRates[ResourceType.Mira] ?? 0;
        }
        action.Value = value;
    }

    private void EvaluateUpgradeFactories(NPCResearchAction action)
    {
        int value = EvaluateCost(action);
        foreach (Tile tile in action.Faction.Territory)
        {
            if (tile.Building.Name == "Factory") // i hate the code  write today
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
