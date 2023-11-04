using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GamePlayScreen
{ 
    private Game _game;
    private SpriteBatch _spriteBatch;
    private TileManager _tilemanager;
    //Tile properties
    private Texture2D _tileTexture;
    int _tileWidth = 64; // Replace with your tile's width
    int _tileHeight = 56; // Replace with your tile's height
    float _tileOffset = 0.02f;

    public GamePlayScreen(Game game, TileManager tileManager)
    {
        _tilemanager = tileManager;
        _game = game;
    }

    public void LoadContent()
    {        
       _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        _tileTexture = _game.Content.Load<Texture2D>("sprites/hexTile");
    }

    public void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        // Render all parts of the screen
        DrawBackground();
        DrawGrid();

        _spriteBatch.End();
    }

    private void DrawBackground()
    {
        _game.GraphicsDevice.Clear(Color.DarkMagenta);
    }

    //TODO: take this into its own class
    private void DrawGrid()
    {
        foreach ((Vector2 coordinates, Tile tile) in _tilemanager.Grid)
        {
            // Calculate the position to draw the tile on the screen
            // Offset for X depends on the row we're drawing
            float drawX = (coordinates.X * _tileWidth + coordinates.Y % 2 * _tileWidth / 2) * (1 +_tileOffset);
            // Offset for Y depends on the column
            float drawY = coordinates.Y * _tileHeight * 0.75f * (1 + _tileOffset );

            // Determine the rectangle where the tile texture will be drawn
            Rectangle destinationRectangle = new Rectangle((int)drawX, (int)drawY, _tileWidth, _tileHeight);

            // Draw the tile
            _spriteBatch.Draw(_tileTexture, destinationRectangle, Color.White);
        }
    }

}