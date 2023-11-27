public class NPCResearchAction : INPCAction
{
    public double Value {get; set;}
    public Faction Faction {get; private set;}
    public ResearchUpgrade Upgrade {get; private set;}

    public NPCResearchAction(Faction faction, ResearchUpgrade upgrade)
    {
        Faction = faction;
        Upgrade = upgrade;
    }
    public void ExecuteAction()
    {
        Faction.ActivateUprade(Upgrade);
    }
}