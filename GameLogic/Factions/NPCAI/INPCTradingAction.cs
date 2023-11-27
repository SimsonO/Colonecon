public interface INPCTradingAction : INPCAction
{
    Faction TradingPartner {get; }
    int Amount {get; }
}