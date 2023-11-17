using System.Collections.Generic;
using Colonecon;

public class FactionManager
{
    public List<NPCFaction> NPCFactions;
    public Player Player {get; private set;}

    public FactionManager(ColoneconGame game)
    {
        Player = new Player("Earth", GlobalColorScheme.PlayerColor, ResourceType.TerraSteel);
        NPCFaction Zy = new NPCFaction("Zy", GlobalColorScheme.NPCFaction1Color, ResourceType.Zytha, game.TileManager, game.BuildOptionLoader.BuildOptions);
        NPCFaction Vorex = new NPCFaction("Vorex", GlobalColorScheme.NPCFaction2Color, ResourceType.Vorixium, game.TileManager, game.BuildOptionLoader.BuildOptions);
        NPCFactions = new List<NPCFaction>
        {
            Zy,
            Vorex
        };
    }
}