using System.Collections.Generic;

public class FactionManager
{
    public List<NPCFaction> NPCFactions;
    public Player Player {get; private set;}

    public FactionManager()
    {
       
        Player = new Player("Earth", GlobalColorScheme.PlayerColor, ResourceType.TerraSteel);
        NPCFaction Zy = new NPCFaction("Zy", GlobalColorScheme.NPCFaction1Color, ResourceType.Zytha);
        NPCFaction Vorex = new NPCFaction("Vorex", GlobalColorScheme.NPCFaction2Color, ResourceType.Vorixium);
        NPCFactions = new List<NPCFaction>
        {
            Zy,
            Vorex
        };
    }
}