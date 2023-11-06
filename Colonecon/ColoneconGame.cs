﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;

namespace Colonecon;

public class ColoneconGame : Game
{
    private GraphicsDeviceManager _graphics;
    private TileManager _tileManager;
    private GamePlayScreen _gamePlayScreen;

    public ColoneconGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

         _tileManager = new TileManager(new Point(10,8));
        _gamePlayScreen = new GamePlayScreen(this, _tileManager);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
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
