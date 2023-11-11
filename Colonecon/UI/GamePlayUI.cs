using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using Colonecon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

public class GamePlayUI
{
    
    private BuildOptionLoader _buildOptionHandler;
    private ColoneconGame _game;
    private Dictionary<Building, Button> _buildingButtons = new Dictionary<Building, Button>(); 
    //State that changes how Input on the GameScreen is handled
    public Building SelectedBuilding {get; private set;}
    //UIElements
    private Desktop _desktop;
    private Window _infoWindow;
    private Label _InfoMessage;

    
    public GamePlayUI(ColoneconGame game, BuildOptionLoader buildOptionHandler)
    {
        _buildOptionHandler = buildOptionHandler;
        _game = game;
        LoadContent();
        TileMapManager.OnBuildingPlaced += ClearBuildingSelection;
        TileMapManager.OnNotEnoughResources += InformAboutMissingRessources;
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
            Height = 64 // Set the height of the header
        };

        // Create the footer panel
        var footer = new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
            Height = 64 // Set the height of the footer
        };

        // Create buttons for header and footer
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
      

        var endTurnButton = new Button
        {
            Content = new Label
            {
                Text = "End Turn",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Right
        };

        //Create Building Menu
        var buildingOptions = new HorizontalStackPanel();
        buildingOptions.Spacing = 16;
        buildingOptions.HorizontalAlignment = HorizontalAlignment.Center;

        foreach(Building building in _buildOptionHandler.BuildOptions)
        {
            Texture2D texture = _game.Content.Load<Texture2D>(building.SpritePath);
            var buildingSprite = new Image()
            {
                Width = 64,
                Height = 64,
                Color = GlobalColorScheme.PlayerColor,
                Renderable = new TextureRegion(texture),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,             
            };
            
            
            Button button = new Button()
            {                
                Content = buildingSprite,
                Background = null,
                OverBackground = null,
                VerticalAlignment = VerticalAlignment.Center                  
            };
            _buildingButtons.Add(building, button);
            buildingOptions.Widgets.Add(_buildingButtons.Values.Last<Button>());
            button.TouchDown += (s,Building) => SetSelectedBuilding(building);
        }

        // Add the buttons to the header and footer
        header.Widgets.Add(menuButton);
        footer.Widgets.Add(buildingOptions);
        footer.Widgets.Add(endTurnButton);

        //InfoWindow
        _InfoMessage = new Label
        {
            Text = "message"
        };

        Button AcknowlegdeMessageButton = new Button
        {
            Content = new Label
            {
                Text = "Ok",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Center
        };

        VerticalStackPanel WindowContent = new VerticalStackPanel();

        WindowContent.Widgets.Add(_InfoMessage);
        WindowContent.Widgets.Add(AcknowlegdeMessageButton);
        
        _infoWindow = new Window
        {
            Title = "title",
            Content = WindowContent,
            DragDirection = DragDirection.None
        };    
       
        AcknowlegdeMessageButton.TouchDown += (s,a) => _infoWindow.Close(); 
        // Add the header and footer to the desktop
        _desktop.Widgets.Add(header);
        _desktop.Widgets.Add(footer);

        _infoWindow.ShowModal(_desktop);

        
    }
    private void SetSelectedBuilding(Building building)
    {
        ClearBuildingSelection();
        SelectedBuilding = building;
        MarkSelectedBuildingButton(building);
    }

    private void MarkSelectedBuildingButton(Building building)
    {
        Button button = _buildingButtons[building];
        button.Border = new SolidBrush(GlobalColorScheme.AccentColor);
        button.BorderThickness = new Thickness(4, 4);
    }

    private void ClearBuildingSelection()
    {
        SelectedBuilding = null;
        foreach (Button button in _buildingButtons.Values)
        {
            ResetBuildingButton(button);
        }
    }

    private void ResetBuildingButton(Button button)
    {
        //Set Default Values here
        button.Border = null;
    }
    
    public void Draw(GameTime gameTime)
    {
        // Draw the Myra UI
        _desktop.Render();
    }

    private void InformAboutMissingRessources(Building building, Faction faction)
    {
        if(faction == _game.FactionManager.Player)
        {
            Dictionary<ResourceType, int> missingResources = building.BuildCost;
            foreach(ResourceType resource in missingResources.Keys)
            {
                missingResources[resource] -= faction.RessourceStock[resource];
            }
            string message = "You cannot build " + building.Name + ". You are missing: ";
            foreach(ResourceType resource in missingResources.Keys)
            {
                message += "\n " + resource + ": " + missingResources[resource] + "\n ";
            }
            DisplayMessage("Not enough Ressources", message);
        }
    }

    private void DisplayMessage (string title, string message)
    {
        _infoWindow.Title = title;
        _InfoMessage.Text = message;
        _infoWindow.ShowModal(_desktop);
    }

}