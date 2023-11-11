using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


public class BuildOptionLoader
{
    public List<Building> BuildOptions {get; private set;}    
    public BuildOptionLoader ()
    {
        BuildOptions = LoadBuildingData();
    }

    private List<Building> LoadBuildingData()
    {
        string json = File.ReadAllText("../Colonecon/Content/data/buildings.json");
        BuildingOptions buildingOptions = JsonSerializer.Deserialize<BuildingOptions>(json);
        return buildingOptions.Buildings;
    }
}