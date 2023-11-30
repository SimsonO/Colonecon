using System;
using System.Collections.Generic;
using Colonecon;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

public class Header
{
    private Label _turnCounter;
    private Label _miraAmount;
    private HorizontalStackPanel _highscore;
    private Panel _header;
    private TurnManager _turnManager;
    private Desktop _desktop;
    private ColoneconGame _game;

    public delegate void RestartGameEventHandler();
    public static event RestartGameEventHandler OnRestartGame;

    public Header(TurnManager turnManager, Desktop desktop, ColoneconGame game)
    {
        _turnManager = turnManager;
        _desktop = desktop;
        _game = game;

        TurnManager.OnTurnEndedEvent += UpdateTurnCounter;        
        TileMapManager.OnPlayerLandingBasePlaced += ShowHeader;
        Faction.OnResourcesChanged += UpdateHighscore;
    }

    public Panel CreateHeader(int height)
    {
        _header = new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Height = height,
            Visible = false
        };
        _highscore = CreateHighscore();
        _turnCounter = new Label
        {
            Text = "Turn " + _turnManager.TurnCounter + "/" + _turnManager.MaxTurns,
            HorizontalAlignment= HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,           
            Margin = new Thickness(32,32)
        };
        
        var menuButton = new Button
        {
            Content = new Label
            {
                Text = "Menu",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            },
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(32,32),
            Background = new SolidBrush(GlobalColorScheme.PrimaryColor),
            Width = 128
        };
        _header.Widgets.Add(_highscore);
        _header.Widgets.Add(_turnCounter);
        _header.Widgets.Add(menuButton);

        Window menu = new Window
        {
            Title = "Menu",
            Background = new SolidBrush(GlobalColorScheme.BackgroundColor),
            Height  = 256,
            Width = 256
        };
        Button restartGameButton = new Button
        {
            Content = new Label
            {
                Text = "Restart",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidBrush(GlobalColorScheme.PrimaryColor),
            Width = 128
        };
        restartGameButton.TouchDown += (s, a) => menu.Close();
        restartGameButton.TouchDown += (s, a) => OnRestartGame?.Invoke();
        restartGameButton.TouchDown += (s, a) => UpdateTurnCounter(0);
        Button endGameButton = new Button
        {
            Content = new Label
            {
                Text = "Close Game",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidBrush(GlobalColorScheme.PrimaryColor),
            Width = 128
        };
        endGameButton.TouchDown += (s, a) => _game.Exit();
        
        VerticalStackPanel menuPanel = new VerticalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 16
        };
          
        menuPanel.Widgets.Add(restartGameButton);
        menuPanel.Widgets.Add(endGameButton);

        
        menuButton.TouchDown += (s, Desktop) => menu.ShowModal(_desktop);
        
        menu.Content = menuPanel;

        return _header;
    }

    private HorizontalStackPanel CreateHighscore()
    {
        HorizontalStackPanel miraStock = new HorizontalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,      
            Margin = new Thickness(32,32),
            Spacing = 8
        };
        Label highscore = new Label
        {
            Text = "Highscore: ",
            VerticalAlignment = VerticalAlignment.Center,
        };
        Dictionary<ResourceType, int> playerResources = _game.FactionManager.Player.ResourceStock;
        String spritePath = "sprites/" + ResourceType.Mira;
        Texture2D texture = _game.Content.Load<Texture2D>(spritePath);
        Image resourceSprite = new Image()
        {
            Width = 16,
            Height = 16,
            Color = GlobalColorScheme.PrimaryColor,
            Renderable = new TextureRegion(texture),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        _miraAmount = new Label
        {
            Text = playerResources[ResourceType.Mira].ToString(),
            VerticalAlignment = VerticalAlignment.Center
        };
        miraStock.Widgets.Add(highscore);        
        miraStock.Widgets.Add(_miraAmount);
        miraStock.Widgets.Add(resourceSprite);
        return miraStock;
    }

    public void UpdateTurnCounter(int newTurnCounter)
    {
        _turnCounter.Text = newTurnCounter+ "/" + _turnManager.MaxTurns;
    }

    public void UpdateHighscore(Faction faction)
    {
        _miraAmount.Text = _game.FactionManager.Player.ResourceStock[ResourceType.Mira].ToString(); 
    }

    private void ShowHeader()
    {
        _header.Visible = true;
        TileMapManager.OnPlayerLandingBasePlaced -= ShowHeader;
    }



}