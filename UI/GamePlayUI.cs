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

    
    public GamePlayUI(ColoneconGame game, TurnManager turnManager)
    {
        _game = game;
        _desktop = new Desktop();

        GamePlayDashboard = new Dashboard(_game);
        GamePlayFooter = new Footer(game,this, turnManager, _desktop);
        _gamePlayHeader = new Header(turnManager, _desktop);
        TradeMenu = new TradeMenu(_game.FactionManager, this);        
        
        Panel header = _gamePlayHeader.CreateHeader();
        VerticalStackPanel dashbord = GamePlayDashboard.CreateDashboard();
        HorizontalStackPanel footer = GamePlayFooter.CreateFooter();
        CreateStartingMethod();

        _desktop.Widgets.Add(_startingMessage);
        _desktop.Widgets.Add(header);
        _desktop.Widgets.Add(dashbord);
        _desktop.Widgets.Add(TradeMenu.TradeMenuPanel);
        _desktop.Widgets.Add(footer);

        TileMapManager.OnPlayerLandingBasePlaced += HideStartingMessage;
        Header.OnRestartGame += Reset;
    }
    public void Reset()
    {
        _startingMessage.Visible = true;
        TileMapManager.OnPlayerLandingBasePlaced += HideStartingMessage;
    }
    private void CreateStartingMethod()
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

    public void Draw(GameTime gameTime)
    {
        // Draw the Myra UI
        _desktop.Render();
    }
}