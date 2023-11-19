using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


public class BuildOptionLoader
{
    public List<Building> BuildOptions {get; private set;}
    public Building StartingBase {get; private set;}
    public BuildOptionLoader ()
    {
        BuildOptions = LoadBuildingData();
        StartingBase = LoadStartingBase();
    }

    private List<Building> LoadBuildingData()
    {
        string json = File.ReadAllText("../Colonecon/Content/data/buildings.json");
        BuildingOptions buildingOptions = JsonSerializer.Deserialize<BuildingOptions>(json);
        return buildingOptions.Buildings;
    }
    private Building LoadStartingBase()
    {
        Building LandingBase = new Building
        {
            Name = "LandingBase",
            SpritePath = "sprites/Rocket",
            BuildLimit = 1,
            BuildCost = {},
            ConsumptionRates = {},
            ProductionRates = {}
        };
        return LandingBase;
    }



}