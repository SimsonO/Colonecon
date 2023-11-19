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

    
    public GamePlayUI(ColoneconGame game, TurnManager turnManager)
    {
        _game = game;
        GamePlayDashboard = new Dashboard(_game);
        GamePlayFooter = new Footer(game,this, turnManager);
        _gamePlayHeader = new Header(turnManager);
        TradeMenu = new TradeMenu(_game.FactionManager, this);
        LoadContent();
    }
    public void LoadContent()
    {
        // Create the desktop that will hold your UI
        _desktop = new Desktop();
       

        Panel header = _gamePlayHeader.CreateHeader();
        VerticalStackPanel dashbord = GamePlayDashboard.CreateDashboard();
        HorizontalStackPanel footer = GamePlayFooter.CreateFooter();

        Label startingMessage = new Label
        {
            Text = "Choose your landing spot!",
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(0,16),
        };

        _desktop.Widgets.Add(startingMessage);
        _desktop.Widgets.Add(header);
        _desktop.Widgets.Add(dashbord);
        _desktop.Widgets.Add(TradeMenu.TradeMenuPanel);
        _desktop.Widgets.Add(footer);

    }

    public void Draw(GameTime gameTime)
    {
        // Draw the Myra UI
        _desktop.Render();
    }

}