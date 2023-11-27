// These variables should be declared in your class scope
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class TestHexfield
{
    private MouseState _previousMouseState;
    private string _hexCoordinatesDisplay = "";
    private SpriteFont _font; // Ensure you load a font in LoadContent

    private TileMapView _tileMapView;
    private Game _game;
    private SpriteBatch _spriteBatch;
    public TestHexfield(Game game, TileMapView tileMapView, SpriteBatch spriteBatch)
    {
        _game = game;
        _tileMapView = tileMapView;
        _spriteBatch = spriteBatch;
        _font = _game.Content.Load<SpriteFont>("fonts/File");
    }

    public void Update(GameTime gameTime)
    {
        MouseState currentMouseState = Mouse.GetState();

        // Check if the left mouse button was newly clicked
        if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            // Convert the mouse position to hex coordinates
            Point hexCoords = _tileMapView.ScreenToHex(new Point(currentMouseState.X, currentMouseState.Y));

            // Update the display string with the new coordinates
            _hexCoordinatesDisplay = $"Hex {hexCoords.X}  {hexCoords.Y}";
        }

        // Save the current state for comparison in the next frame
        _previousMouseState = currentMouseState;
    }

    public void DrawHexCoordinates(GameTime gameTime)
    {

        // Draw other game elements here

        // Draw the hex coordinates on the screen
        if (!string.IsNullOrEmpty(_hexCoordinatesDisplay))
        {
            _spriteBatch.DrawString(_font, _hexCoordinatesDisplay, new Vector2(10, 10), Color.White);
        }

    }
}
