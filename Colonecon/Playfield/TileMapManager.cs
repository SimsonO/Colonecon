using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class TileMapManager
{
    public Point MapSize {get; private set;}
    public Dictionary<Point, Tile> TileMap {get; private set;}
    private Random _rnd;

    public delegate void BuildingPlacedEventHandler();
    public static event BuildingPlacedEventHandler OnBuildingPlaced;
    public delegate void NotEnoughResourcesEventHandler(Building building, Faction faction);
    public static event NotEnoughResourcesEventHandler OnNotEnoughResources;

    public TileMapManager(Point mapSize)
    {
        MapSize = mapSize;
        TileMap = new Dictionary<Point, Tile>();
        _rnd = new Random();
        GenerateTileMap();
    }

    public void GenerateTileMap()
    {
        TileMap.Clear();
        for (int i = 0; i < MapSize.Y;i++)
        {
            //for odd rows we want 1 tile more then for even
           for (int j = 0; j < MapSize.X - i % 2; j++)
            {
                Point coordinates = new Point(j,i);
                Tile tile = new Tile( GetRandomMiraDeposit());
                TileMap.Add(coordinates, tile);
            }
        }

    }

    private int GetRandomMiraDeposit()
    {
        int miraDeposit = Math.Max(0,_rnd.Next(-10,10))*100;
        return miraDeposit;
    }

    public void BuildOnTile(Point tileCoordiante, Building building, Faction faction)
    {
        if (TileMap[tileCoordiante].Building is null)
        {
            if(faction.SubtractResources(building.BuildCost))
            {
                TileMap[tileCoordiante].OccupyTile(faction);
                TileMap[tileCoordiante].PlaceBuilding(building);
                OnBuildingPlaced?.Invoke();
            }
            else
            {
                OnNotEnoughResources?.Invoke(building, faction);
            }
        }       
    }


}