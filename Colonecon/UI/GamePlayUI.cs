using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

public class GamePlayUI
{
    private Desktop _desktop;
    private BuildOptionHandler _buildOptionHandler;
    private Game _game;

    public GamePlayUI(Game game, BuildOptionHandler buildOptionHandler)
    {
        _buildOptionHandler = buildOptionHandler;
        _game = game;
        LoadContent();
    }
    public void LoadContent()
    {
        // Create the desktop that will hold your UI
        _desktop = new Desktop();

        // Create the header panel
        var header = new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            Height = 50 // Set the height of the header
        };

        // Create the footer panel
        var footer = new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            Height = 50 // Set the height of the footer
        };

        // Create buttons for header and footer
        var menuButton = new TextButton
        {
            Text = "Menu",
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var endTurnButton = new TextButton
        {
            Text = "End Turn",
            HorizontalAlignment = HorizontalAlignment.Right
        };

        //Create Building Menu
        var buildingOptions = new HorizontalStackPanel();
        buildingOptions.Spacing = 16;
        buildingOptions.HorizontalAlignment = HorizontalAlignment.Center;

        List<Button> buildingButton = new List<Button>(); 

        foreach(Building building in _buildOptionHandler.BuildOptions.Keys)
        {
            Button button = new Button();
            Texture2D texture = _game.Content.Load<Texture2D>(_buildOptionHandler.BuildOptions[building].SpritePath);
            button.Background = new NinePatchRegion(texture, new Rectangle(10, 10, 50, 50), 
                                        new Thickness {Left = 2, Right = 2, 
                                                       Top = 2, Bottom = 2});
            buildingButton.Add(button);
            buildingOptions.Widgets.Add(buildingButton.Last<Button>());
        }

        // Add the buttons to the header and footer
        header.Widgets.Add(menuButton);
        footer.Widgets.Add(buildingOptions);
        footer.Widgets.Add(endTurnButton);

        // Add the header and footer to the desktop
        _desktop.Widgets.Add(header);
        _desktop.Widgets.Add(footer);

        
    }
    
    public void Draw(GameTime gameTime)
    {
        // Draw the Myra UI
        _desktop.Render();
    }

}