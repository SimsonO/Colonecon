using System.Collections.Generic;

public class Building
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string SpritePath { get; set; }
    public int BuildLimit { get; set; }
    public Dictionary<ResourceType, int> ProductionRates { get; set; }
    public Dictionary<ResourceType, int> ConsumptionRates { get; set; }
    public Dictionary<ResourceType, int> BuildCost { get; set; }

    /*public Building(string name, string spriteBatch, Dictionary<ResourceType, int> productionRates, Dictionary<ResourceType, int> consumptionRates, Dictionary<ResourceType, int> buildCost )
    {
        Name = name;
        SpritePath = spriteBatch;
        ProductionRates = productionRates;
        ConsumptionRates = consumptionRates;
        BuildCost = buildCost;
    }*/
}