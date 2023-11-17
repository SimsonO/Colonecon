using Myra.Graphics2D.UI;

public class Header
{
    private Label _turnCounter;
    private TurnManager _turnManager;

    public Header(TurnManager turnManager)
    {
        _turnManager = turnManager;
        
        TurnManager.OnTurnEndedEvent += UpdateTurnCounter;
    }
    public Panel CreateHeader()
    {

        var header = new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Height = 64 // Set the height of the header
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
        header.Widgets.Add(_turnCounter);
        header.Widgets.Add(menuButton);
        return header;
    }
    private void UpdateTurnCounter(int newTurnCounter)
    {
        _turnCounter.Text = newTurnCounter+ "/" + _turnManager.MaxTurns;
    }

}