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
    public TurnManager(FactionManager factionManager)
    {
        TurnCounter = 0;
        MaxTurns = 20;
        _factionManager = factionManager;
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
        faction.RunTurn();
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
        //Calculate Highscore
        //Show Endscreen or send Endgame Event
    }

}