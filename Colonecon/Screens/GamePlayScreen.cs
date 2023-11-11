using Colonecon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GamePlayScreen
{ 
    private ColoneconGame _game;
    private SpriteBatch _spriteBatch;
    private TileMapManager _tilemanager;
    private TileMapView _tileMapView;
    private TestHexfield _testHexfield;   //delete at some point
    private GamePlayUI _gamePlayUI;
    private BuildOptionLoader _buildOptionHandler;
    private TileMapInputHandler _tileMapInputHandler;

    public GamePlayScreen(ColoneconGame game, TileMapManager tileManager)
    {
        _tilemanager = tileManager;
        _game = game;
    }

    public void LoadContent()
    {        
        _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        _buildOptionHandler = new BuildOptionLoader();
        _tileMapView = new TileMapView(_spriteBatch, _tilemanager,_game, _buildOptionHandler);
        _testHexfield = new TestHexfield(_game, _tileMapView, _spriteBatch);        
        _gamePlayUI = new GamePlayUI(_game, _buildOptionHandler);
        _tileMapInputHandler = new TileMapInputHandler(_game, _tileMapView, _tilemanager, _gamePlayUI);
    }

    public void Update(GameTime gameTime)
    {
         // Update the UI library input
        //_gamePlayView.Update(gameTime);
        _testHexfield.Update(gameTime);
        _tileMapInputHandler.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();

        // Render all parts of the screen
        DrawBackground();
       _tileMapView.DrawTileMap(gameTime);
       _testHexfield.DrawHexCoordinates(gameTime);

        _spriteBatch.End();
        //Render UI
        _gamePlayUI.Draw(gameTime);
    }

    private void DrawBackground()
    {
        _game.GraphicsDevice.Clear(GlobalColorScheme.BackgroundColor);
    }
}