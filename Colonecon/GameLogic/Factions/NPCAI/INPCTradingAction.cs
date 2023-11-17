public interface INPCTradingAction : INPCAction
{
    ResourceType Resource {get; set;}
    int Amount {get; set;}
}