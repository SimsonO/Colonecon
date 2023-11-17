using Colonecon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GamePlayScreen
{ 
    private ColoneconGame _game;
    private SpriteBatch _spriteBatch;
    private TileMapView _tileMapView;
    private TurnManager _turnManager;
    private TestHexfield _testHexfield;   //delete at some point
    private GamePlayUI _gamePlayUI;
    private TileMapInputHandler _tileMapInputHandler;

    public GamePlayScreen(ColoneconGame game)
    {
        _game = game;
    }

    public void LoadContent()
    {        
        _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        _tileMapView = new TileMapView(_spriteBatch,_game);
        _testHexfield = new TestHexfield(_game, _tileMapView, _spriteBatch);      
        _turnManager = new TurnManager(_game.FactionManager);  
        _gamePlayUI = new GamePlayUI(_game, _turnManager);
        _tileMapInputHandler = new TileMapInputHandler(_game, _tileMapView, _game.TileManager, _gamePlayUI.GamePlayFooter);
    }

    public void Update(GameTime gameTime)
    {
         // Update the UI library input
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