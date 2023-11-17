using System;
using System.Collections.Generic;
using Colonecon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class TileMapView
{
    private SpriteBatch _spriteBatch;
    private TileMapManager _tileManager;
    private Texture2D _tileTexture;
    private Texture2D _tileBorderTexture;
    private Dictionary<Building, Texture2D> _buildingSprites;
    private int _tileWidth = 64; // Replace with your tile's width
    private int _tileHeight = 64; // Replace with your tile's height
    private Point _initialOffset;


    public TileMapView(SpriteBatch spriteBatch, ColoneconGame game)
    {
        _spriteBatch = spriteBatch;
        _tileManager = game.TileManager;
        _tileTexture = game.Content.Load<Texture2D>("sprites/hexTile");
        _tileBorderTexture = game.Content.Load<Texture2D>("sprites/hexTileBorder");
        _buildingSprites = new Dictionary<Building, Texture2D>();
        foreach (Building building in game.BuildOptionLoader.BuildOptions)
        {
            Texture2D buildingTexture = game.Content.Load<Texture2D>(building.SpritePath);
            _buildingSprites.Add(building, buildingTexture);
        }
        _initialOffset = GetOffsetToCenterTileMap(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
    }

    private Point GetOffsetToCenterTileMap(int screenWidth, int screenHeight)
    {
        int totalMapWidth = _tileManager.MapSize.X * _tileWidth ; //Subtrakt one offset because we don't start with tileoffset
        int totalMapHeight = (int)(_tileManager.MapSize.Y * _tileHeight * 0.75f + 0.25f*_tileHeight);// Additional quarter tile height for the last row

        Point initialOffset = new Point((screenWidth - totalMapWidth) / 2, (screenHeight - totalMapHeight) / 2);
        return initialOffset;
        
    }
    public void DrawTileMap(GameTime gameTime)
    {        
        foreach ((Point coordinates, Tile tile) in _tileManager.TileMap)
        {
            DrawTile(coordinates, tile);        
            if (tile.Building is not null)
            {
                DrawBuilding(coordinates, tile);
            }
            if (tile.TileOwner is not null)
            {
                DrawTileOwnership(coordinates, tile);
            }
        }
    }

    private void DrawTile(Point coordinates, Tile tile)
    {
            // Calculate the position to draw the tile on the screen
            // Offset for X depends on the row we're drawing
            int drawX = coordinates.X * _tileWidth + coordinates.Y % 2 * _tileWidth / 2 + _initialOffset.X ;
            // Offset for Y depends on the column
            int drawY = coordinates.Y * ((int)(_tileHeight * 0.75f)) + _initialOffset.Y;
            // Determine the rectangle where the tile texture will be drawn
            Rectangle destinationRectangle = new Rectangle((int)drawX, (int)drawY, _tileWidth, _tileHeight);
            //get the colorTint based of MiraDeposit
            float colorIntensity = 1f - 0.4f * (float)Math.Round((float)tile.MiraStartDeposit/1000,1);
            Color tileColor = GlobalColorScheme.AdjustIntensity(GlobalColorScheme.TileColor, colorIntensity);            
            // Draw the tile
            _spriteBatch.Draw(_tileTexture, destinationRectangle, tileColor);
    }
    private void DrawBuilding(Point coordinates, Tile tile)
    {
        // Calculate the position to draw the tile on the screen
        // Offset for X depends on the row we're drawing
        int drawX = coordinates.X * _tileWidth + coordinates.Y % 2 * _tileWidth / 2 + _initialOffset.X ;
        // Offset for Y depends on the column
        int drawY = coordinates.Y * ((int)(_tileHeight * 0.75f)) + _initialOffset.Y;
        // Determine the rectangle where the tile texture will be drawn
        Rectangle destinationRectangle = new Rectangle((int)drawX, (int)drawY, _tileWidth, _tileHeight);           
        // Draw the tile
        _spriteBatch.Draw(_buildingSprites[tile.Building], destinationRectangle, tile.TileOwner.Color);
    }

    private void DrawTileOwnership(Point coordinates, Tile tile)
    {
        // Calculate the position to draw the tile on the screen
        // Offset for X depends on the row we're drawing
        int drawX = coordinates.X * _tileWidth + coordinates.Y % 2 * _tileWidth / 2 + _initialOffset.X ;
        // Offset for Y depends on the column
        int drawY = coordinates.Y * ((int)(_tileHeight * 0.75f)) + _initialOffset.Y;
        // Determine the rectangle where the tile texture will be drawn
        Rectangle destinationRectangle = new Rectangle((int)drawX, (int)drawY, _tileWidth, _tileHeight);
        // Draw the tile
        _spriteBatch.Draw(_tileBorderTexture, destinationRectangle, tile.TileOwner.Color);
    }


    public Point ScreenToHex(Point screenPoint)
    {
        /*Credit to Troyseph on Stackoverflow: https://stackoverflow.com/questions/7705228/hexagonal-grids-how-do-you-find-which-hexagon-a-point-is-in */
        
        // Adjust the point by the initial offset
        screenPoint -= _initialOffset;
        if (screenPoint.X >= 0 && screenPoint.Y >= 0) // check first i click is in TileMap (currently only for top left, becasue on those sides it was buggy)
        {
            // Calculate the size of the edges of the hex
            float hexHeight = _tileHeight * 0.75f ;
            float hexWidth = _tileWidth;
            float halfWidth = _tileWidth / 2;
            float pointHeigt = _tileHeight * 0.25f;
            float pointGradient = pointHeigt / halfWidth;
            
            
            // Determine the approximate column and row
            int column;
            int row = (int)(screenPoint.Y / hexHeight);
            // Correct for the staggering of the rows
            bool rowIsOdd = row % 2 == 1;
            if(rowIsOdd && screenPoint.X >= halfWidth)
            {
                column = (int)((screenPoint.X - halfWidth) / hexWidth);
            }
            else if(rowIsOdd && screenPoint.X < halfWidth)//no Tile here
            {
                column = -1;
            }
            else
            {
                column = (int)(screenPoint.X / hexWidth);
            }

            // Work out the position of the point relative to the box it is in
            double relY = screenPoint.Y - (row * hexHeight);
            double relX;

            if (rowIsOdd)
            {
                relX = screenPoint.X - (column * hexWidth) - halfWidth;
            }            
            else
            {
                relX = screenPoint.X - (column * hexWidth);
            }
            // Work out if the point is above either of the hexagon's top edges
            if (relY < (-pointGradient * relX) + pointHeigt) // LEFT edge
            {
                row--;
                if (!rowIsOdd)
                    column--;
            }
            else if (relY < (pointGradient * relX) - pointHeigt) // RIGHT edge
            {
                row--;
                if (rowIsOdd)
                    column++;
            }
            return new Point(column, row);
        }
        else
        {
            return new Point(-1,-1);
        }
    }  
        
}