using System;
using System.Collections.Generic;
using System.Linq;

public class NPCAI
{
    private NPCFaction _faction;
    private TileMapManager _tileMapManager;
    private FactionManager _factionManager;
    private List<Building> _buildingOptions;
    private ActionEvaluator _actionEvaluator;

    public NPCAI(NPCFaction faction, TileMapManager tileMapManager, List<Building> buildingOptions, FactionManager factionManager)
    {
        _faction = faction;
        _tileMapManager = tileMapManager;
        _factionManager = factionManager;
        _buildingOptions = buildingOptions;
        _actionEvaluator = new ActionEvaluator();
    }

    public void TakeNPCaction()
    {
        ExecuteBestAction();
    }

    private void ExecuteBestAction()
    {
        List<INPCAction> possibleActions = GetPossibleActions();
        if(possibleActions.Count > 0)
        {
            GetHighestValueAction(possibleActions).ExecuteAction();
            ExecuteBestAction();
        }
        else
        {
            _faction.EndTurn();
        }
        
    }

    private List<INPCAction> GetPossibleActions()
    {
        List<INPCAction> actions = GetPossibleBuildActions();
        actions.AddRange(GetPossibleTradeActions());
        actions.AddRange(GetResearchActions());
        return actions;
    }

    private List<INPCAction> GetPossibleBuildActions()
    {
        List<INPCAction> buildingActions = new List<INPCAction>();
        foreach(Building building in _buildingOptions)
        {
            if(_faction.EnoughResources(building.BuildCost))
            {
                foreach(Tile tile in _faction.Territory)
                {
                    if(tile.Building is null)
                    {
                        NPCBuildingAction action = new NPCBuildingAction(_faction, building, tile, _tileMapManager);
                        _actionEvaluator.Evaluate(action);
                        buildingActions.Add(action);
                    }
                }
            }
        }
        return buildingActions;
    }

    private List<INPCAction> GetPossibleTradeActions()
    {
        List<INPCAction> tradingActions = new List<INPCAction>();
        foreach(Faction faction in _factionManager.NPCFactions)
        {
            if(faction == _faction)
            {
                if(_faction.ResourceStock[ResourceType.Mira]>=faction.FactionResourcePrice * 5)
                {
                    NPCBuyFromHome action = new NPCBuyFromHome(_faction, 5);
                    _actionEvaluator.Evaluate(action);
                    tradingActions.Add(action);
                }
            }
            else
            {
                if(_faction.ResourceStock[ResourceType.Mira]>=faction.TradePrice * 5 && faction.AvailableTradeAmountFactionResource >= 5)
                {
                    NPCTradingAction action = new NPCTradingAction(_faction, 5, faction);
                    _actionEvaluator.Evaluate(action);
                    tradingActions.Add(action);
                }
            }
        }
        if(_faction.ResourceStock[ResourceType.Mira]>=_factionManager.Player.TradePrice * 5 && _factionManager.Player.AvailableTradeAmountFactionResource >= 5)
                {
                    NPCTradingAction action = new NPCTradingAction(_faction, 5, _factionManager.Player);
                    _actionEvaluator.Evaluate(action);
                    tradingActions.Add(action);
                }
        return tradingActions;
    }

    private List<INPCAction> GetResearchActions()
    {
        List<INPCAction> actions = new List<INPCAction>();
        if(_faction.ResearchEnabled)
        {
            
            foreach(ResearchUpgrade upgrade in _faction.ResearchUpgrades)
            {
                if(_faction.EnoughResources(upgrade.UpgradeCost))
                {
                    NPCResearchAction action = new NPCResearchAction(_faction,upgrade);
                    actions.Add(action);
                }
            }
        }
        return actions;
    }

    private INPCAction GetHighestValueAction(List<INPCAction> actions)
    {
        return actions.MaxBy(action => action.Value);
    }
}