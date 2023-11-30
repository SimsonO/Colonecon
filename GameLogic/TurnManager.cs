using System;
using System.Linq;
using System.Runtime.CompilerServices;

public class TurnManager
{
    public int TurnCounter {get; private set;}
    public int MaxTurns {get; private set;}
    private FactionManager _factionManager;
    public delegate void TurnEndedEventHandler(int newTurnCounter);
    public static event TurnEndedEventHandler OnTurnEndedEvent;

    public delegate void GameEndedEventHandler(int highscore);
    public static event GameEndedEventHandler OnGameEndedEvent;
    public TurnManager(FactionManager factionManager)
    {
        TurnCounter = 0;
        MaxTurns = 10;
        _factionManager = factionManager;

        GamePlayUI.OnRestartGame += Reset;
        Header.OnRestartGame += Reset;
    }

    public void Reset()
    {
        TurnCounter = 0;
    }

    public void EndPlayerTurn()
    {
        if( TurnCounter == MaxTurns)
        {
            _factionManager.Player.EndTurn();
            EndGame();
        }
        else
        {
            _factionManager.Player.EndTurn();
            TurnCounter++;
            OnTurnEndedEvent?.Invoke(TurnCounter);
            RunNPCTurn(0);
        } 
    }

    private void RunNPCTurn(int index)
    {
        NPCFaction faction = _factionManager.NPCFactions[index];
        if(!faction.HasLeftThePlanet)
        {
            faction.RunTurn();
        }
        if(index + 1 < _factionManager.NPCFactions.Count)
        {
            RunNPCTurn(index + 1);
        }
        else
        {
            //addLogic to let Player play again
        }
    }


    public void EndGame()
    {
        int highscore = _factionManager.Player.ResourceStock[ResourceType.Mira];
        OnGameEndedEvent?.Invoke(highscore);
    }
}