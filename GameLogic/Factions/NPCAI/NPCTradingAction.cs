public class NPCTradingAction : INPCTradingAction
{
    public double Value {get; set;}
    public Faction Faction {get; private set;}
    public Faction TradingPartner {get; private set;}
    public int Amount {get; private set;}

    public NPCTradingAction(Faction faction, int amount, Faction tradingPartner)
    {
        Faction = faction;
        Amount = amount;
        TradingPartner = tradingPartner;
    }
    public void ExecuteAction()
    {
        if(TradingPartner.AvailableTradeAmountFactionResource >= Amount)
        {
            if(Faction.BuyResources(TradingPartner.FactionResource, Amount, TradingPartner.TradePrice))
            {
                TradingPartner.SellFactionResource(Amount);
            }
        }
    }
}