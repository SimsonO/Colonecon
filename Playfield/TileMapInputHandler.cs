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
    private Tile _hoveredTile;

    public delegate void TileHoveredEventHandler(Tile tile);
    public static event TileHoveredEventHandler OnTileHovered;
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

        // Convert the mouse position to hex coordinates
        Point hexCoords = _tileMapView.ScreenToHex(new Point(currentMouseState.X, currentMouseState.Y));
        // Check if the left mouse button was newly clicked
        if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            if(_uiFooter.SelectedBuilding is not null)
            {
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
        else
        {
            if(_tileMapManager.TileMapByCoordinates.Keys.Contains<Point>(hexCoords)) 
            {
                if(_hoveredTile != _tileMapManager.TileMapByCoordinates[hexCoords] )
                {
                    _hoveredTile = _tileMapManager.TileMapByCoordinates[hexCoords];
                    OnTileHovered?.Invoke(_hoveredTile);
                }
            }
            else
            {
                _hoveredTile = null;
            }
        }

        // Save the current state for comparison in the next frame
        _previousMouseState = currentMouseState;
    }

}