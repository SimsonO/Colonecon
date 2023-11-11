using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra;

namespace Colonecon;

public class ColoneconGame : Game
{
    public GraphicsDeviceManager Graphics;
    private TileMapManager _tileManager;
    private GamePlayScreen _gamePlayScreen;

    public FactionManager FactionManager {get; private set;}

    public ColoneconGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

         _tileManager = new TileMapManager(new Point(10,8));
        _gamePlayScreen = new GamePlayScreen(this, _tileManager);
    }

    protected override void Initialize()
    {
        FactionManager = new FactionManager();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
         // Initialize Myra
        MyraEnvironment.Game = this;
        _gamePlayScreen.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
        _gamePlayScreen.Update(gameTime); 
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _gamePlayScreen.Draw(gameTime);        
    }
}
