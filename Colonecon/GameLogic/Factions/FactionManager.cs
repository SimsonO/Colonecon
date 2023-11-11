using System.Collections.Generic;

public class FactionManager
{
    public List<Faction> Factions;
    public Player Player {get; private set;}

    public FactionManager()
    {
       
        Player = new Player("Earth", GlobalColorScheme.PlayerColor);
        Factions = new List<Faction>
        {
            Player
        };
    }
}