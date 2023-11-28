using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class NPCFaction : Faction
{
    private int _tradeThreshhold; //Only Resources exceeding the threshhold are up for trade
    private NPCAI _ai;
    private Building _landingbase;
    private TileMapManager _tileMapManager;
    private int _miraThreshhold;
    private int _turnCounter;
    public bool HasLeftThePlanet = false;

    public delegate void FactionLeftEventHandler(Faction faction);
    public static event FactionLeftEventHandler OnFactionLeftEvent;
    public NPCFaction(string name, Color color, ResourceType factionResource, TileMapManager tileMapManager, BuildOptionLoader buildOptionLoader, FactionManager factionManager) : base(name, color, factionResource)
    {
        _ai = new NPCAI(this, tileMapManager, buildOptionLoader.BuildOptions, factionManager);
        _tileMapManager = tileMapManager;
        _landingbase = buildOptionLoader.StartingBase;
        _miraThreshhold = 250;

        TileMapManager.OnPlayerLandingBasePlaced += PlaceLandingBase; //The first building is always the starting base. After that each npc builds it startingBase
        TurnManager.OnTurnEndedEvent += UpdateTurnCounter;
    }

    private void PlaceLandingBase()
    {
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
        _tradeThreshhold = 5; //add some Logic here to account for more need in faction
    }

    private void CalculateTradePrice()
    {
        TradePrice = 5; // add some Log here to account supply/demand
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
        CheckIfFactionLeaves();
    }

    private void CheckIfFactionLeaves()
    {
        if(_turnCounter > 5 && (MiraDepositInTerritory() < _miraThreshhold))
        {
            HasLeftThePlanet = true;
            DestroyAllBuilding();
            OnFactionLeftEvent?.Invoke(this);
        }
    }

    private int MiraDepositInTerritory()
    {
        int miraDepositInTerritory = 0;
        foreach(Tile tile in Territory)
        {
            if((tile.Building is null) || (tile?.Building?.ProductionRates?.ContainsKey(ResourceType.Mira) ?? false))
            {
                miraDepositInTerritory += tile.MiraCurrentDeposit;
            }
        }
        return miraDepositInTerritory;
    }

    private void DestroyAllBuilding()
    {
        foreach(Tile tile in Territory)
        {
            tile.DestroyBuilding();
        }
    }
    private void UpdateTurnCounter(int newTurnCounter)
    {
        _turnCounter = newTurnCounter;
    }
}