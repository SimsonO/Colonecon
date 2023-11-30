using Microsoft.Xna.Framework;

public class Player : Faction
{
    public Player(string name, Color color, ResourceType factionResource) : base(name, color, factionResource)
    {

    }

    public override void Reset()
    {
        base.Reset();
        BoostPlayerStart();
    }

    public void BoostPlayerStart()
    {
        AddRessources(ResourceType.Communium, 5);
    }

    public bool IncreaseAvailableTradeAmount(int amount)
    {
        if(ResourceStock[FactionResource] >= amount)
        {
            AvailableTradeAmountFactionResource += amount;
            SubtractResources(FactionResource, amount);
            return true;
        }        
        return false;
    }

    public bool DecreaseAvailableTradeAmount(int amount)
    {
        if(AvailableTradeAmountFactionResource >= amount)
        {
            AvailableTradeAmountFactionResource -= amount;
            AddRessources(FactionResource,amount);
            return true;
        }
        return false;
    }
}