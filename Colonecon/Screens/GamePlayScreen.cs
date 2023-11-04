using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GamePlayScreen
{ 
    private Game _game;
    private SpriteBatch _spriteBatch;
    private TileManager _tilemanager;
    private TileMapView _tileMapView;
    private TestHexfield _testHexfield;
    //Tile properties

    public GamePlayScreen(Game game, TileManager tileManager)
    {
        _tilemanager = tileManager;
        _game = game;
    }

    public void LoadContent()
    {        
        _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        _tileMapView = new TileMapView(_spriteBatch, _tilemanager,_game);
        _testHexfield = new TestHexfield(_game, _tileMapView, _spriteBatch);
    }

    public void Update(GameTime gameTime)
    {
        _testHexfield.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        // Render all parts of the screen
        DrawBackground();
       _tileMapView.DrawTileMap(gameTime);
       _testHexfield.DrawHexCoordinates(gameTime);

        _spriteBatch.End();
    }

    private void DrawBackground()
    {
        _game.GraphicsDevice.Clear(GlobalColorScheme.BackgroundColor);
    }
}