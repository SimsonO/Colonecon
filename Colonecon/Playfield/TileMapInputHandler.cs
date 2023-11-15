using System.Linq;
using Colonecon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class TileMapInputHandler
{
    private MouseState _previousMouseState;
    private TileMapView _tileMapView;
    private TileMapManager _tileMapManager;
    private GamePlayUI _gamePlayUI;
    private ColoneconGame _game;
    public TileMapInputHandler(ColoneconGame game, TileMapView tileMapView, TileMapManager tileMapManager, GamePlayUI gamePlayUI)
    {
        _game = game;
        _tileMapView = tileMapView;
        _gamePlayUI = gamePlayUI;
        _tileMapManager = tileMapManager;

    }

    public void Update(GameTime gameTime)
    {
        MouseState currentMouseState = Mouse.GetState();

        // Check if the left mouse button was newly clicked
        if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            // Convert the mouse position to hex coordinates
            Point hexCoords = _tileMapView.ScreenToHex(new Point(currentMouseState.X, currentMouseState.Y));
            if(_gamePlayUI.SelectedBuilding is not null)
            {
                if(_tileMapManager.TileMap.Keys.Contains<Point>(hexCoords)) 
                {
                    _tileMapManager.BuildOnTile(hexCoords, _gamePlayUI.SelectedBuilding, _game.FactionManager.Player);
                }
                else
                {
                    _gamePlayUI.ClearBuildingSelection();
                }
            }

        }

        // Save the current state for comparison in the next frame
        _previousMouseState = currentMouseState;
    }

}