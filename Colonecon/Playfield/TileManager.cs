using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class TileManager
{
    private Vector2 _gridSize;
    public Dictionary<Vector2, Tile> Grid {get; private set;}

    public TileManager(Vector2 gridSize)
    {
        _gridSize = gridSize;
        Grid = new Dictionary<Vector2, Tile>();
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        Grid.Clear();
        for (int i = 0; i < _gridSize.X;i++)
        {
            int j = 0;
            //for odd rows we want 1 tile more then for even
            if(i % 2 == 0 )
            {
                j++;
            }
            while(j < _gridSize.Y)
            {
                Vector2 coordinates = new Vector2(j,i);
                Tile tile = new Tile( GetRandomTileType());
                Grid.Add(coordinates, tile);
                j++;
            }
        }

    }

    private TileType GetRandomTileType()
    {
        return TileType.Water; //TODO: add Logic to get tiletype. use noise or sth. to get rare tiletypes
    }


}