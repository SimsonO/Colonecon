using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class NPCFaction : Faction
{
    private int _tradeThreshhold; //Only Resources exceeding the threshhold are up for trade
    private NPCAI _ai;
    private Building _landingbase;
    private TileMapManager _tileMapManager;
    public NPCFaction(string name, Color color, ResourceType factionResource, TileMapManager tileMapManager, BuildOptionLoader buildOptionLoader, FactionManager factionManager) : base(name, color, factionResource)
    {
        _ai = new NPCAI(this, tileMapManager, buildOptionLoader.BuildOptions, factionManager);
        _tileMapManager = tileMapManager;
        _landingbase = buildOptionLoader.StartingBase;

        TileMapManager.OnPlayerLandingBasePlaced += PlaceLandingBase; //The first building is always the starting base. After that each npc builds it startingBase  
    }

    private void PlaceLandingBase()
    {
        TileMapManager.OnPlayerLandingBasePlaced -= PlaceLandingBase;
        Point landingCoordinates = _tileMapManager.GetStartingCoordinatesNPC();
        _tileMapManager.BuildOnTile(landingCoordinates,_landingbase,this);
    }  

    private void CalculateTradeAmount()
    {
        CalculateTradeTheshhold();
        AvailableTradeAmountFactionResource = Math.Max(ResourceStock[FactionResource] - _tradeThreshhold,0);
    }
    private void CalculateTradeTheshhold()
    {   
        _tradeThreshhold = 10; //add some Logic here to account for more need in faction
    }

    private void CalculateTradePrice()
    {
        TradePrice = 10; // add some Log here to account supply/demand
    }

    public void RunTurn()
    {
        _ai.TakeNPCaction();
    }
    public override void EndTurn()
    {
        CalculateTradeAmount();
        CalculateTradePrice();
        base.EndTurn();
    }
}