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
    private Footer _uiFooter;
    private ColoneconGame _game;
    public TileMapInputHandler(ColoneconGame game, TileMapView tileMapView, TileMapManager tileMapManager, Footer gamePlayFooter)
    {
        _game = game;
        _tileMapView = tileMapView;
        _uiFooter = gamePlayFooter;
        _tileMapManager = tileMapManager;

    }

    public void Update(GameTime gameTime)
    {
        MouseState currentMouseState = Mouse.GetState();

        // Check if the left mouse button was newly clicked
        if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if(_uiFooter.SelectedBuilding is not null)
            {
                // Convert the mouse position to hex coordinates
                Point hexCoords = _tileMapView.ScreenToHex(new Point(currentMouseState.X, currentMouseState.Y));
                if(_tileMapManager.TileMapByCoordinates.Keys.Contains<Point>(hexCoords)) 
                {
                    _tileMapManager.BuildOnTile(hexCoords, _uiFooter.SelectedBuilding, _game.FactionManager.Player);
                }
                else
                {
                    _uiFooter.ClearBuildingSelection();
                }
            }

        }

        // Save the current state for comparison in the next frame
        _previousMouseState = currentMouseState;
    }

}