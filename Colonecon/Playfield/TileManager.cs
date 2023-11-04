using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class TileManager
{
    public Point MapSize {get; private set;}
    public Dictionary<Point, Tile> TileMap {get; private set;}
    private Random _rnd;

    public TileManager(Point mapSize)
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


}