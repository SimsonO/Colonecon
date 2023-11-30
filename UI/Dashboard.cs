using System;
using System.Collections.Generic;
using Colonecon;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

public class Dashboard
{
    private Dictionary<ResourceType, Label> _playerResourceDisplay;
    private VerticalStackPanel _dashboard;
    private Panel _buildingInformationPanel;
    private Panel _tileInformationPanel;
    private Label _tileInfo;
    private Panel _messagePanel;
    private Label _message;

    private ColoneconGame _game;

    public Dashboard(ColoneconGame game)
    {
        _game = game;        
        _playerResourceDisplay = new Dictionary<ResourceType, Label>();

        Faction.OnResourcesChanged += UpdatePlayerResources;
        TileMapManager.OnBuildingPlaced += UpdatePlayerResources;
        TileMapManager.OnPlayerLandingBasePlaced += ShowDashboard;
        TileMapInputHandler.OnTileHovered += ShowTileInfo;
    }
    
    public VerticalStackPanel CreateDashboard()
    {
        _dashboard = new VerticalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Visible = false,
            Margin = new Thickness(16, (int)(_game.GraphicsDevice.Viewport.Height * 0.1)),
            Spacing = 124,
            Background = new SolidBrush(GlobalColorScheme.BackgroundColor),
            Width = (int)(_game.GraphicsDevice.Viewport.Width * 0.1)
        };
        _dashboard.Widgets.Add(CreateResourceDisplay());
        _dashboard.Widgets.Add(CreateInfoContainer());
        return _dashboard;
    }

    private VerticalStackPanel CreateInfoContainer()
    {
        VerticalStackPanel infoContainer = new VerticalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Spacing = 32
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
        _tileInfo = new Label
        {
            Text = "MiraDeposit: 0"
        };
        _tileInformationPanel.Widgets.Add(_tileInfo);
        _messagePanel.Widgets.Add(_message);        
        infoContainer.Widgets.Add(_tileInformationPanel);
        infoContainer.Widgets.Add(_buildingInformationPanel);
        infoContainer.Widgets.Add(_messagePanel);


        return infoContainer;
    }

    private VerticalStackPanel CreateResourceDisplay()
    {
        Dictionary<ResourceType, int> playerResources = _game.FactionManager.Player.ResourceStock;
        
        var resourceDisplay = new VerticalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Spacing = 16            
        };
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
            int produce = 0;
            if(_game.FactionManager.Player.ResourceProduce.ContainsKey(resource))
            {
                produce = _game.FactionManager.Player.ResourceProduce[resource];
            }
            int consume = 0;
            if(_game.FactionManager.Player.ResourceConsume.ContainsKey(resource))
            {
                consume = _game.FactionManager.Player.ResourceConsume[resource];
            }
            
            Label resourceAmount = new Label
            {
                Text = playerResources[resource] + "| +" + produce + "| -" + consume
            };
            resourceStock.Widgets.Add(resourceSprite);
            resourceStock.Widgets.Add(resourceAmount);
            _playerResourceDisplay.Add(resource,resourceAmount);
            
            resourceDisplay.Widgets.Add(resourceStock); 
        }
        
        return resourceDisplay;
    }
    private void UpdatePlayerResources(Faction faction)
    {
        if(faction == _game.FactionManager.Player)
        {
            Dictionary<ResourceType, int> playerResources = _game.FactionManager.Player.ResourceStock;
            foreach(ResourceType resource in playerResources.Keys)
            {
                int produce = 0;
                if(_game.FactionManager.Player.ResourceProduce.ContainsKey(resource))
                {
                    produce = _game.FactionManager.Player.ResourceProduce[resource];
                }
                int consume = 0;
                if(_game.FactionManager.Player.ResourceConsume.ContainsKey(resource))
                {
                    consume = _game.FactionManager.Player.ResourceConsume[resource];
                }
                _playerResourceDisplay[resource].Text = playerResources[resource] + "| +" + produce + "| -" + consume;
            }
        }
    }
    
    public void ShowBuildingInfo(Building building)
    {
        HideInfoPanels();
        _buildingInformationPanel.Widgets.Clear();
        _buildingInformationPanel.Widgets.Add(CreateBuilingInfoContent(building));
        _buildingInformationPanel.Visible = true;
    }

    private VerticalStackPanel CreateBuilingInfoContent(Building building)
    {
        VerticalStackPanel buildingInfo = new VerticalStackPanel
        {
            Spacing = 16
        };

        Label Name = new Label
        {
            Text = building.Name
        };
        buildingInfo.Widgets.Add(Name);
        Label Description = new Label
        {
            Text = building.Description,
            Wrap = true
        };
        buildingInfo.Widgets.Add(Description);
        if((building.ProductionRates?.Count ?? 0) > 0)
        {
            Label Production = new Label
            {
                Text = "Production rates"
            };
            buildingInfo.Widgets.Add(Production);
            foreach(ResourceType resource in building.ProductionRates.Keys)
            {
                HorizontalStackPanel resourceProduce = new HorizontalStackPanel();
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
                    Text = building.ProductionRates[resource].ToString()
                };
                resourceProduce.Widgets.Add(resourceSprite);
                resourceProduce.Widgets.Add(resourceAmount);
                
                buildingInfo.Widgets.Add(resourceProduce); 
            }
        }
        if((building.ConsumptionRates?.Count ?? 0) > 0)
        {
            Label Consumption = new Label
            {
                Text = "Consumption rates"
            };
            buildingInfo.Widgets.Add(Consumption);
            foreach(ResourceType resource in building.ConsumptionRates.Keys)
            {
                HorizontalStackPanel resourceConsumption = new HorizontalStackPanel();
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
                    Text = building.ConsumptionRates[resource].ToString()
                };
                resourceConsumption.Widgets.Add(resourceSprite);
                resourceConsumption.Widgets.Add(resourceAmount);
                
                buildingInfo.Widgets.Add(resourceConsumption); 
            }
        }     
        return buildingInfo;
    }

    public void HideBuildingInfo()
    {
        _buildingInformationPanel.Visible = false;
    }

    public void ShowTileInfo(Tile tile)
    {
       
        _tileInfo.Text = "Mira deposit: " + tile.MiraCurrentDeposit;
        if(tile.Building is not null)
        {
            ShowBuildingInfo(tile.Building);
            _tileInformationPanel.Visible = true;
        }
        else
        {
            HideInfoPanels();
            _tileInformationPanel.Visible = true;
        }
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

    private void ShowDashboard()
    {
        _dashboard.Visible = true;
        TileMapManager.OnPlayerLandingBasePlaced -= ShowDashboard;
    }
}