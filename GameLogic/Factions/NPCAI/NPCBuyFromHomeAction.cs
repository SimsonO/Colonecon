public class NPCBuyFromHome : INPCTradingAction
{
    public double Value {get; set;}
    public Faction Faction {get; private set;}
    public Faction TradingPartner {get; private set;}
    public int Amount {get; private set;}

    public NPCBuyFromHome(Faction faction, int amount)
    {
        Faction = faction;
        TradingPartner = faction;
        Amount = amount;
    }
    public void ExecuteAction()
    {
        Faction.TradeFromHome(Amount);
    }
}