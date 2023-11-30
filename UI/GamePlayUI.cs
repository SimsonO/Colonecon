using System;
using System.Collections.Generic;
using Colonecon;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;

public class GamePlayUI
{
    private ColoneconGame _game;
    //UIElements
    private Desktop _desktop;
    private Header _gamePlayHeader;
    public Dashboard GamePlayDashboard;
    public Footer GamePlayFooter;
    public TradeMenu TradeMenu;
    private Label _startingMessage;
    private Window _factionLeftInfoWindow;
    private Label _factionLeftInfoContent;
    private Window _gameOverWindow;
    private Label _gameOverText;

    public delegate void RestartGameEventHandler();
    public static event RestartGameEventHandler OnRestartGame;
    public GamePlayUI(ColoneconGame game, TurnManager turnManager)
    {
        _game = game;
        _desktop = new Desktop();

        GamePlayDashboard = new Dashboard(_game);
        GamePlayFooter = new Footer(game,this, turnManager, _desktop);
        _gamePlayHeader = new Header(turnManager, _desktop, _game);
        TradeMenu = new TradeMenu(_game.FactionManager, this, _desktop, _game);        
        
        Panel header = _gamePlayHeader.CreateHeader((int)(_game.GraphicsDevice.Viewport.Height * 0.1));
        VerticalStackPanel dashbord = GamePlayDashboard.CreateDashboard();
        HorizontalStackPanel footer = GamePlayFooter.CreateFooter();
        CreateStartingMessage();
        CreateFactionLeaveWindow();
        CreateGameOverWindow();

        _desktop.Widgets.Add(_startingMessage);
        _desktop.Widgets.Add(header);
        _desktop.Widgets.Add(dashbord);
        _desktop.Widgets.Add(footer);

        TileMapManager.OnPlayerLandingBasePlaced += HideStartingMessage;
        NPCFaction.OnFactionLeftEvent += FactionLeaveWindow;
        Header.OnRestartGame += Reset;
        TurnManager.OnGameEndedEvent += ShowGameOverWindow;
    }

    public void Reset()
    {
        _startingMessage.Visible = true;
        TileMapManager.OnPlayerLandingBasePlaced += HideStartingMessage;
    }
    private void CreateStartingMessage()
    {
        _startingMessage = new Label
        {
            Text = "Choose your landing spot!",
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(0, 16),
        };
    }

    private void HideStartingMessage()
    {
        _startingMessage.Visible = false;
        TileMapManager.OnPlayerLandingBasePlaced -= HideStartingMessage;
    }

    private void CreateFactionLeaveWindow()
    {
        _factionLeftInfoWindow = new Window
        {
            Title = "A faction Left"
        };
        _factionLeftInfoContent = new Label
        {
            Text = ""
        };
        _factionLeftInfoWindow.Content = _factionLeftInfoContent;
    }

    private void FactionLeaveWindow(Faction faction)
    {
        _factionLeftInfoContent.Text = "The " + faction.Name + " weren't profitable anymore and left the Planet";
        _factionLeftInfoWindow.ShowModal(_desktop);
    }

    private void CreateGameOverWindow()
    {
        _gameOverWindow = new Window
        {
            Title = "Game Over"
        };
        VerticalStackPanel _gameOverContent = new VerticalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 32
        };
        _gameOverText = new Label
        {
            Text = ""
        };
        Button restartGameButton = new Button
        {
            Content = new Label
            {
                Text = "Try again",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        restartGameButton.TouchDown += (s, a) => _gameOverWindow.Close();
        restartGameButton.TouchDown += (s, a) => OnRestartGame?.Invoke();
         restartGameButton.TouchDown += (s, a) => Reset();
        restartGameButton.TouchDown += (s, a) => _gamePlayHeader.UpdateTurnCounter(0);
        _gameOverContent.Widgets.Add(_gameOverText);
        _gameOverWindow.Content = _gameOverContent;
    }

    private void ShowGameOverWindow(int highscore)
    {
        _gameOverText.Text = "Your Highcore is " + highscore;
        _gameOverWindow.ShowModal(_desktop);
    }


    public void Draw(GameTime gameTime)
    {
        // Draw the Myra UI
        _desktop.Render();
    }
}