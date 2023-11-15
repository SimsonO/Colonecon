using System;
using System.Collections.Generic;
using System.Linq;
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
    private TurnManager _turnManager;
    private Dictionary<Building, Button> _buildingButtons = new Dictionary<Building, Button>(); 
    public Building SelectedBuilding {get; private set;}
    //UIElements
    private Desktop _desktop;
    private Label _turnCounter;
    private Dictionary<ResourceType, Label> _playerResourceDisplay;
    private Panel _buildingInformationPanel;
    private Panel _tileInformationPanel;
    private Panel _messagePanel;
    private Label _message;
    private Dictionary<NPCFaction, VerticalStackPanel> _tradeMenuContent;
    private HorizontalStackPanel _tradeMenu;

    
    public GamePlayUI(ColoneconGame game, BuildOptionLoader buildOptionHandler, TurnManager turnManager)
    {
        _buildOptionHandler = buildOptionHandler;
        _game = game;
        _turnManager = turnManager;

        TileMapManager.OnBuildingPlaced += ClearBuildingSelection;
        TileMapManager.OnNotEnoughResources += InformAboutMissingRessources;
        Faction.OnResourcesChanged += UpdatePlayerResources;
        TurnManager.OnTurnEndedEvent += UpdateTurnCounter;

        LoadContent();
    }
    public void LoadContent()
    {
        // Create the desktop that will hold your UI
        _desktop = new Desktop();
        _playerResourceDisplay = new Dictionary<ResourceType, Label>();
        _tradeMenuContent = new Dictionary<NPCFaction, VerticalStackPanel>();

        Panel header = CreateHeader();
        VerticalStackPanel dashbord = CreateDashboard();
        HorizontalStackPanel footer = CreateFooter();
        _tradeMenu = CreateTradeMenu();

        _desktop.Widgets.Add(header);
        _desktop.Widgets.Add(dashbord);
        _desktop.Widgets.Add(_tradeMenu);
        _desktop.Widgets.Add(footer);
    }

    private Panel CreateHeader()
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

    private VerticalStackPanel CreateDashboard()
    {
        var dashbord = new VerticalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
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
    
    private void ShowBuildingInfo(Building building)
    {
        HideInfoPanels();
        _buildingInformationPanel.Visible = true;
    }

    private void HideBuildingInfo()
    {
        _buildingInformationPanel.Visible = false;
    }
    private HorizontalStackPanel CreateTradeMenu()
    {
        HorizontalStackPanel tradeMenu = new HorizontalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Visible = false,
            Background = new SolidBrush(GlobalColorScheme.BackgroundColor)
        };
        foreach(NPCFaction faction in _game.FactionManager.NPCFactions)
        {
            VerticalStackPanel factionTradeMenu = CreateFactionTradeMenu(faction);
            _tradeMenuContent.Add(faction, factionTradeMenu);
            tradeMenu.Widgets.Add(factionTradeMenu);
        }
        Button closeMenu = new Button
        {
            Content = new Label
            {
                Text = "close"
            },
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        closeMenu.TouchDown += (s,a) => tradeMenu.Visible = false;
        tradeMenu.Widgets.Add(closeMenu);
        return tradeMenu;
    }

    private VerticalStackPanel CreateFactionTradeMenu(NPCFaction faction)
    {
        VerticalStackPanel factionTradeMenu = new VerticalStackPanel
        {

        };
        Label factionName = new Label
        {
          Text = faction.Name  
        };
        Label factionResource = new Label
        {
          Text = faction.FactionResource.ToString()  
        };
        Label TradeAmount = new Label
        {
          Text = "Available: " + faction.AvailableTradeAmountFactionResource
        };
        Label TradePrice = new Label
        {
          Text = "Price: " + faction.TradePrice
        };
        Button buy1factionResource = new Button
        {
            Content = new Label
            {
                Text = "Buy 1 " + faction.FactionResource
            }
        };
        buy1factionResource.TouchDown += (s, Faction) => BuyFactionResource(faction, 1);
        Button buy5factionResource = new Button
        {
            Content = new Label
            {
                Text = "Buy 5 " + faction.FactionResource
            }
        };
        buy5factionResource.TouchDown += (s, Faction) => BuyFactionResource(faction, 5);
        factionTradeMenu.Widgets.Add(factionName);
        factionTradeMenu.Widgets.Add(factionResource);
        factionTradeMenu.Widgets.Add(TradeAmount);
        factionTradeMenu.Widgets.Add(TradePrice);
        factionTradeMenu.Widgets.Add(buy1factionResource);
        factionTradeMenu.Widgets.Add(buy5factionResource);    
        
        return factionTradeMenu;
    }

    private void BuyFactionResource(NPCFaction faction, int tradeAmount)
    {
        if(faction.AvailableTradeAmountFactionResource >= tradeAmount)
        {
            if(_game.FactionManager.Player.BuyResources(faction.FactionResource, tradeAmount, faction.TradePrice))
            {
                faction.SellFactionResource(tradeAmount);
            }
            else
            {
                DisplayMessage("","You cannot afford this");
            }  
        }
        else
        {
            DisplayMessage("","Not enough " + faction.FactionResource + " available");
        }        
    }

    private HorizontalStackPanel CreateFooter()
    {
        HorizontalStackPanel footer = new HorizontalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        Button endTurnButton = new Button
        {
            Content = new Label
            {
                Text = "End Turn",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        endTurnButton.TouchDown += (s, a) => _turnManager.EndPlayerTurn();
        Button openTradeMenu = new Button
        {
            Content = new Label
            {
                Text = "Trade Menu",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        openTradeMenu.TouchDown += (s,a) => _tradeMenu.Visible = true;
        footer.Widgets.Add(CreateBuildingOptions());
        footer.Widgets.Add(openTradeMenu);
        footer.Widgets.Add(endTurnButton);
        return footer;
    }
    private HorizontalStackPanel CreateBuildingOptions()
    {
        var buildingOptions = new HorizontalStackPanel
        {
            Spacing = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidBrush(GlobalColorScheme.BackgroundColor)
        };

        foreach (Building building in _buildOptionHandler.BuildOptions)
        {
            HorizontalStackPanel buildingContainer = new HorizontalStackPanel
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Spacing = 8
            };
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
                VerticalAlignment = VerticalAlignment.Center,                
                BorderThickness = new Thickness(4, 4)
            };
            _buildingButtons.Add(building, button);
            buildingContainer.Widgets.Add(_buildingButtons.Values.Last<Button>());
            button.TouchDown += (s, Building) => SetSelectedBuilding(building);
            button.MouseEntered += (s, Building) => ShowBuildingInfo(building);
            button.MouseLeft += (s, a) => HideBuildingInfo();

          
            var buildingCost = new HorizontalStackPanel
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Spacing = 16            
            };
            var buildingCostColumn1 = new VerticalStackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var buildingCostColumn2 = new VerticalStackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            int i = 0;
            foreach(ResourceType resource in building.BuildCost.Keys)
            {         
                HorizontalStackPanel resourceCost = new HorizontalStackPanel();
                String spritePath = "sprites/" + resource;
                Texture2D textureRes = _game.Content.Load<Texture2D>(spritePath);
                Image resourceSprite = new Image()
                {
                    Width = 16,
                    Height = 16,
                    Color = GlobalColorScheme.PrimaryColor,
                    Renderable = new TextureRegion(textureRes),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Label resourceAmount = new Label
                {
                    Text = building.BuildCost[resource].ToString()
                };
                resourceCost.Widgets.Add(resourceSprite);
                resourceCost.Widgets.Add(resourceAmount);
                if(i%2 == 0)
                {
                    buildingCostColumn1.Widgets.Add(resourceCost);
                }
                else
                {
                    buildingCostColumn2.Widgets.Add(resourceCost);
                }
                i++; 
            }
            buildingCost.Widgets.Add(buildingCostColumn1);
            buildingCost.Widgets.Add(buildingCostColumn2);
            buildingContainer.Widgets.Add(buildingCost);
            buildingOptions.Widgets.Add(buildingContainer);
        }
        return buildingOptions;
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
    }
    public void ClearBuildingSelection()
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
                message += "\n " + resource + ": " + missingResources[resource];
            }
            DisplayMessage("Not enough Ressources", message);
        }
    }

    private void DisplayMessage (string title, string message)
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
    public void Draw(GameTime gameTime)
    {
        // Draw the Myra UI
        _desktop.Render();
    }

}