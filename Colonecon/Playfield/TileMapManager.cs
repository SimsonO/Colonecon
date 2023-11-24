using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Microsoft.Xna.Framework;

public class TileMapManager
{
    public Point MapSize {get; private set;}
    public Dictionary<Point, Tile> TileMapByCoordinates {get; private set;}
    public Dictionary<Tile, Point> TileMapByTiles {get; private set;}
    private Random _rnd;

    public delegate void BuildingPlacedEventHandler(Faction faction);
    public static event BuildingPlacedEventHandler OnBuildingPlaced;
    public delegate void PlayerLandingBasePlacedEventHandler();
    public static event PlayerLandingBasePlacedEventHandler OnPlayerLandingBasePlaced;
    public delegate void NotEnoughResourcesEventHandler(Building building, Faction faction);
    public static event NotEnoughResourcesEventHandler OnNotEnoughResources;

    public TileMapManager(Point mapSize)
    {
        MapSize = mapSize;
        TileMapByCoordinates = new Dictionary<Point, Tile>();
        TileMapByTiles = new Dictionary<Tile, Point>();
        _rnd = new Random();
        GenerateTileMap();

        Header.OnRestartGame += Reset;
    }

    public void Reset()
    {
        GenerateTileMap();
    }

    public void GenerateTileMap()
    {
        TileMapByCoordinates.Clear();
        TileMapByTiles.Clear();
        for (int i = 0; i < MapSize.Y;i++)
        {
            //for odd rows we want 1 tile more then for even
           for (int j = 0; j < MapSize.X - i % 2; j++)
            {
                Point coordinates = new Point(j,i);
                Tile tile = new Tile( GetRandomMiraDeposit());
                TileMapByCoordinates.Add(coordinates, tile);
                TileMapByTiles.Add(tile, coordinates);
            }
        }

    }

    private int GetRandomMiraDeposit()
    {
        int miraDeposit = Math.Max(0,_rnd.Next(-5,10))*20;
        return miraDeposit;
    }

    public Point GetStartingCoordinatesNPC()
    {
        Point startingCoordinates = new Point(_rnd.Next(MapSize.X - 1),_rnd.Next(MapSize.Y - 1));
        List<Tile> neighbours = GetNeighboringTiles(startingCoordinates);
        bool validStartingpoint = true;
        foreach(Tile tile in neighbours)
        {
            if(tile.TileOwner is not null)
            {
                validStartingpoint = false;
            }
        }
        if(validStartingpoint)
        {
            return startingCoordinates;
        }
        else 
        {
            return GetStartingCoordinatesNPC();
        }
    }

    public void BuildOnTile(Point tileCoordiante, Building building, Faction faction)
    {
        BuildOnTile(TileMapByCoordinates[tileCoordiante], building, faction);
    }

    public void BuildOnTile(Tile tile, Building building, Faction faction)
    {
        if (tile.Building is null)
        {
            if(building.BuildCost is null) // can only be the Landingbase and i know this is hacky
            {
                if(tile.TileOwner is null){tile.OccupyTile(faction);}
                tile.PlaceBuilding(building);                
                OccupyNeighbours(TileMapByTiles[tile], faction);
                if(faction is Player)
                {
                    OnPlayerLandingBasePlaced?.Invoke();
                }                
            }
            else if (tile.TileOwner == faction)
            {
                if(faction.SubtractResources(building.BuildCost))
                {
                    tile.PlaceBuilding(building);                
                    OccupyNeighbours(TileMapByTiles[tile], faction);
                    OnBuildingPlaced?.Invoke(faction);
                }
                else
                {
                    OnNotEnoughResources?.Invoke(building, faction);
                }
            }
            else
            {
                //Invoke not in territory here
            }
            
        }       
    }

    private void OccupyNeighbours(Point tileCoordiante, Faction faction)
    {
        List<Tile> neighbours = GetNeighboringTiles(tileCoordiante);
        foreach(Tile tile in neighbours)
        {
            if(tile.TileOwner is null)
            {
                tile.OccupyTile(faction);
            }
        }
    }

    public List<Tile> GetNeighboringTiles(Point tileCoordiante)
    {
        List<Tile> neighbors = new List<Tile>();
        List<Point> neighborCoordinates = new List<Point>();
        
        int column = tileCoordiante.X;
        int row = tileCoordiante.Y;
        // Check if the row is even or odd
        bool isEvenRow = row % 2 == 0;

        // North-East
        neighborCoordinates.Add(new Point(isEvenRow ? column: column + 1, row - 1));
        // East
        neighborCoordinates.Add(new Point(column + 1, row));
        // South-East
        neighborCoordinates.Add(new Point(isEvenRow ? column : column + 1, row + 1));
        // South-West
        neighborCoordinates.Add(new Point(isEvenRow ? column - 1 : column, row + 1));
        // West
        neighborCoordinates.Add(new Point(column - 1, row));
        // North-West
        neighborCoordinates.Add(new Point(isEvenRow ? column - 1 : column, row - 1));
        
        foreach(Point coordinate in neighborCoordinates)
        {
            if(TileMapByCoordinates.ContainsKey(coordinate))
            {
                neighbors.Add(TileMapByCoordinates[coordinate]);
            }
        }
        return neighbors;
    }
}