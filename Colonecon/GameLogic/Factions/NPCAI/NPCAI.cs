using System;
using System.Collections.Generic;
using System.Linq;

public class NPCAI
{
    private NPCFaction _faction;
    private TileMapManager _tileMapManager;
    private List<Building> _buildingOptions;
    private ActionEvaluator _actionEvaluator;

    public NPCAI(NPCFaction faction, TileMapManager tileMapManager, List<Building> buildingOptions)
    {
        _faction = faction;
        _tileMapManager = tileMapManager;
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
        return actions;
    }

    private List<INPCAction> GetPossibleBuildActions()
    {
        List<INPCAction> buildingActions = new List<INPCAction>();
        foreach(Building building in _buildingOptions)
        {
            if(_faction.EnoughResources(building))
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
        return tradingActions;

    }

    private INPCAction GetHighestValueAction(List<INPCAction> actions)
    {
        return actions.MaxBy(action => action.Value);
    }
}