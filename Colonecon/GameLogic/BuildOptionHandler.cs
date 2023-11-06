using System.Collections.Generic;

public class BuildOptionHandler
{
    public Dictionary<Building, BuildingData> BuildOptions {get; private set;}
    
    public BuildOptionHandler ()
    {
        BuildOptions = new Dictionary<Building, BuildingData>();
        
        BuildingData MineData = new BuildingData("sprites/MiraMine");
        BuildOptions.Add(Building.MiraMine, MineData);

        BuildingData LabData = new BuildingData("sprites/ScienceLab");
        BuildOptions.Add(Building.ScienceLab, LabData);
    }
}

public enum Building
{
    PowerPlant,
    MiraMine,
    Factory,
    ScienceLab
}