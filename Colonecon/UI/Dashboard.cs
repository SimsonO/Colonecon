using System;
using System.Collections.Generic;
using Colonecon;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

public class Dashboard
{
    private Dictionary<ResourceType, Label> _playerResourceDisplay;
    private Panel _buildingInformationPanel;
    private Panel _tileInformationPanel;
    private Panel _messagePanel;
    private Label _message;

    private ColoneconGame _game;

    public Dashboard(ColoneconGame game)
    {
        _game = game;        
        _playerResourceDisplay = new Dictionary<ResourceType, Label>();

        Faction.OnResourcesChanged += UpdatePlayerResources;
    }
    
    public VerticalStackPanel CreateDashboard()
    {
        var dashbord = new VerticalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Visible = false,
            Margin = new Thickness(16, 64)
        };
        dashbord.Widgets.Add(CreateResourceDisplay());
        dashbord.Widgets.Add(CreateInfoContainer());
        return dashbord;
    }

    private Panel CreateInfoContainer()
    {
        Panel infoContainer = new Panel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };
        _buildingInformationPanel = new Panel
        {
            Visible = false
        };  
        Label buildingInfo = new Label
        {
            Text = "test"
        };
        _buildingInformationPanel.Widgets.Add(buildingInfo);
        _messagePanel = new Panel
        {
            Visible = false
        };
        _message = new Label
        {
            Text = "Hello",
            Wrap = true
        };
        _tileInformationPanel = new Panel
        {
            Visible = false
        };
        _messagePanel.Widgets.Add(_message);
        infoContainer.Widgets.Add(_buildingInformationPanel);
        infoContainer.Widgets.Add(_messagePanel);
        infoContainer.Widgets.Add(_tileInformationPanel);


        return infoContainer;
    }

    private HorizontalStackPanel CreateResourceDisplay()
    {
        Dictionary<ResourceType, int> playerResources = _game.FactionManager.Player.RessourceStock;
        
        var resourceDisplay = new HorizontalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Spacing = 16            
        };
        var resourceDisplayColumn1 = new VerticalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var resourceDisplayColumn2 = new VerticalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        int i = 0;
        foreach(ResourceType resource in playerResources.Keys)
        {
            HorizontalStackPanel resourceStock = new HorizontalStackPanel();
            String spritePath = "sprites/" + resource;
            Texture2D texture = _game.Content.Load<Texture2D>(spritePath);
            Image resourceSprite = new Image()
            {
                Width = 16,
                Height = 16,
                Color = GlobalColorScheme.PrimaryColor,
                Renderable = new TextureRegion(texture),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Label resourceAmount = new Label
            {
                Text = playerResources[resource].ToString()
            };
            resourceStock.Widgets.Add(resourceSprite);
            resourceStock.Widgets.Add(resourceAmount);
            _playerResourceDisplay.Add(resource,resourceAmount);
            if(i%2 == 0)
            {
                resourceDisplayColumn1.Widgets.Add(resourceStock);
            }
            else
            {
                resourceDisplayColumn2.Widgets.Add(resourceStock);
            }
            i++; 
        }
        resourceDisplay.Widgets.Add(resourceDisplayColumn1);
        resourceDisplay.Widgets.Add(resourceDisplayColumn2);
        return resourceDisplay;
    }
    private void UpdatePlayerResources(Faction faction)
    {
        if(faction == _game.FactionManager.Player)
        {
            Dictionary<ResourceType, int> playerResources = _game.FactionManager.Player.RessourceStock;
            foreach(ResourceType resource in playerResources.Keys)
            {
                _playerResourceDisplay[resource].Text = playerResources[resource].ToString();
            }
        }
        
    }
    
    public void ShowBuildingInfo(Building building)
    {
        HideInfoPanels();
        _buildingInformationPanel.Visible = true;
    }

    public void HideBuildingInfo()
    {
        _buildingInformationPanel.Visible = false;
    }

    public void DisplayMessage (string title, string message)
    {
        _message.Text = message;
        HideInfoPanels();
        _messagePanel.Visible = true;
    }

    private void HideInfoPanels()
    {
        _buildingInformationPanel.Visible = false;
        _messagePanel.Visible = false;
        _tileInformationPanel.Visible = false;
    }
}