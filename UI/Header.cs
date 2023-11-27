using System;
using Myra.Graphics2D.UI;

public class Header
{
    private Label _turnCounter;
    private Panel _header;
    private TurnManager _turnManager;
    private Desktop _desktop;

    public delegate void RestartGameEventHandler();
    public static event RestartGameEventHandler OnRestartGame;

    public Header(TurnManager turnManager, Desktop desktop)
    {
        _turnManager = turnManager;
        _desktop = desktop;
        
        TurnManager.OnTurnEndedEvent += UpdateTurnCounter;        
        TileMapManager.OnPlayerLandingBasePlaced += ShowHeader;
    }

    public Panel CreateHeader()
    {
        _header = new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Height = 64, // Set the height of the header
            Visible = false
        };
        _turnCounter = new Label
        {
            Text = _turnManager.TurnCounter + "/" + _turnManager.MaxTurns,
            HorizontalAlignment= HorizontalAlignment.Center
        };
        
        var menuButton = new Button
        {
            Content = new Label
            {
                Text = "Menu",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        _header.Widgets.Add(_turnCounter);
        _header.Widgets.Add(menuButton);

        Window menu = new Window
        {
            Title = "Menu"
        };
        Button restartGameButton = new Button
        {
            Content = new Label
            {
                Text = "Restart",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        restartGameButton.TouchDown += (s, a) => menu.Close();
        restartGameButton.TouchDown += (s, a) => OnRestartGame?.Invoke();
        menuButton.TouchDown += (s, Desktop) => menu.ShowModal(_desktop);
        menu.Content = restartGameButton;

        return _header;
    }
    private void UpdateTurnCounter(int newTurnCounter)
    {
        _turnCounter.Text = newTurnCounter+ "/" + _turnManager.MaxTurns;
    }

    private void ShowHeader()
    {
        _header.Visible = true;
        TileMapManager.OnPlayerLandingBasePlaced -= ShowHeader;
    }



}