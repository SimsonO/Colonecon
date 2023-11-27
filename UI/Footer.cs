using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using Colonecon;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

public class Footer
{
    
    private ColoneconGame _game;
    private GamePlayUI _ui;
    private Desktop _desktop;
    private TurnManager _turnManager;
    public Building SelectedBuilding {get; private set;}
    private List<Building> _buildOptions;
    private List<Building> _startingBuilding;
    private HorizontalStackPanel _buildOptionPanel;
    private Button _endTurnButton;
    private Button _tradeMenuButton;
    private Dictionary<Building, Button> _buildingButtons = new Dictionary<Building, Button>();
    private Button _upgrades;
    public Footer(ColoneconGame game, GamePlayUI ui, TurnManager turnManager, Desktop desktop)
    {
        _game = game;
        _desktop = desktop;
        _buildOptions = game.BuildOptionLoader.BuildOptions;
        
        _startingBuilding = new List<Building>
        {
            game.BuildOptionLoader.StartingBase
        };
        SelectedBuilding = game.BuildOptionLoader.StartingBase;

        _ui = ui;
        _turnManager = turnManager;

        TileMapManager.OnPlayerLandingBasePlaced += FillBuildingSection;
        TileMapManager.OnPlayerLandingBasePlaced += ShowButtons;
        TileMapManager.OnNotEnoughResources += InformAboutMissingRessources;
        TileMapManager.OnScienceLabBuild +=ShowUpgrades;
        Header.OnRestartGame += Reset;
    }

    public void Reset()
    {
        SelectedBuilding = _game.BuildOptionLoader.StartingBase;
        FillBuildingOptions(_startingBuilding);
        HideButtons();
        TileMapManager.OnPlayerLandingBasePlaced += FillBuildingSection;
        TileMapManager.OnBuildingPlaced -= ClearBuildingSelection;
    }

    public HorizontalStackPanel CreateFooter()
    {
        HorizontalStackPanel footer = new HorizontalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            Spacing  = 32
        };
        _buildOptionPanel = new HorizontalStackPanel()
        {
            Spacing = 8,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidBrush(GlobalColorScheme.BackgroundColor)
        };
        FillBuildingOptions(_startingBuilding);

        VerticalStackPanel buttonPanel = new VerticalStackPanel
        {
            Spacing = 8
        };
         _endTurnButton = new Button
        {
            Content = new Label
            {
                Text = "End Turn",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center
            },
            HorizontalAlignment = HorizontalAlignment.Right,
            Visible = false
        };
        _endTurnButton.TouchDown += (s, a) => _turnManager.EndPlayerTurn();
        _tradeMenuButton = new Button
        {
            Content = new Label
            {
                Text = "Trade Menu",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center               
            },
            HorizontalAlignment = HorizontalAlignment.Right,
            Visible = false
        };
        _tradeMenuButton.TouchDown += (s,a) => _ui.TradeMenu.OpenTradeMenu();
        _upgrades = new Button
        {
            Content = new Label
            {
                Text = "Research",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center               
            },
            HorizontalAlignment = HorizontalAlignment.Right,
            Visible = false
        };
        CreateResearchWindow();
        
        footer.Widgets.Add(_buildOptionPanel);
        buttonPanel.Widgets.Add(_tradeMenuButton);
        buttonPanel.Widgets.Add(_upgrades);
        buttonPanel.Widgets.Add(_endTurnButton);
        footer.Widgets.Add(buttonPanel);
        return footer;
    }
    private void FillBuildingOptions(List<Building> buildOptions)
    {
        _buildOptionPanel.Widgets.Clear();
        _buildingButtons.Clear();
        foreach (Building building in buildOptions)
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
            button.MouseEntered += (s, Building) => _ui.GamePlayDashboard.ShowBuildingInfo(building);
            button.MouseLeft += (s, a) => _ui.GamePlayDashboard.HideBuildingInfo();


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
            if(building.BuildCost is not null)
            {
                foreach (ResourceType resource in building.BuildCost.Keys)
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
                    if (i % 2 == 0)
                    {
                        buildingCostColumn1.Widgets.Add(resourceCost);
                    }
                    else
                    {
                        buildingCostColumn2.Widgets.Add(resourceCost);
                    }
                    i++;
                }
            }
            
            buildingCost.Widgets.Add(buildingCostColumn1);
            buildingCost.Widgets.Add(buildingCostColumn2);
            buildingContainer.Widgets.Add(buildingCost);
            _buildOptionPanel.Widgets.Add(buildingContainer);
            if(!(SelectedBuilding is null))
            {                
                MarkSelectedBuildingButton(SelectedBuilding);
            }
        }
    }

    private void FillBuildingSection()
    {
        TileMapManager.OnPlayerLandingBasePlaced -= FillBuildingSection;
        TileMapManager.OnBuildingPlaced += ClearBuildingSelection;
        SelectedBuilding = null;
        FillBuildingOptions(_buildOptions);
        ClearBuildingSelection();
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
    public void ClearBuildingSelection(Faction faction)
    {
       if(faction == _game.FactionManager.Player)
       {
            ClearBuildingSelection();
       }
    }

    public void ClearBuildingSelection()
    {
        if(SelectedBuilding != _game.BuildOptionLoader.StartingBase)
        {
            SelectedBuilding = null;
            foreach (Button button in _buildingButtons.Values)
            {
                ResetBuildingButton(button);
            }
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
                missingResources[resource] -= faction.ResourceStock[resource];
            }
            string message = "You cannot build " + building.Name + ". You are missing: ";
            foreach(ResourceType resource in missingResources.Keys)
            {
                message += "\n " + resource + ": " + missingResources[resource];
            }
            _ui.GamePlayDashboard.DisplayMessage("Not enough Ressources", message);
        }
    }

    #region Research
    private void CreateResearchWindow()
    {
        Window research = new Window
        {
            Title = "Research"
        };
        VerticalStackPanel researchContent = new VerticalStackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 16
        };
        foreach(ResearchUpgrade upgrade in _game.FactionManager.Player.ResearchUpgrades)
        {
            HorizontalStackPanel upgradePanel = new HorizontalStackPanel
            {
                Spacing = 8
            };
            Label upgradeName = new Label
            {
                Text = upgrade.Name
            };
            upgradePanel.Widgets.Add(upgradeName);
            Label upgradeDescription = new Label
            {
                Text = upgrade.Description
            };
            upgradePanel.Widgets.Add(upgradeDescription);
            VerticalStackPanel upgradeCost = new VerticalStackPanel
            {
               
            };
            if(upgrade.UpgradeCost is not null)
            {
                foreach (ResourceType resource in upgrade.UpgradeCost.Keys)
                {
                    HorizontalStackPanel upgradeCostPanel = new HorizontalStackPanel();
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
                        Text = upgrade.UpgradeCost[resource].ToString()
                    };
                    upgradeCostPanel.Widgets.Add(resourceSprite);
                    upgradeCostPanel.Widgets.Add(resourceAmount);
                    upgradeCost.Widgets.Add(upgradeCostPanel);
                }
            }
            upgradePanel.Widgets.Add(upgradeCost);
            Button upgradeButton = new Button
            {
                Content = new Label
                {
                    Text = "research",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
                
            };            
            upgradeButton.TouchDown += (s, ResearchUpgrade) => _game.FactionManager.Player.ActivateUprade(upgrade);
            upgradePanel.Widgets.Add(upgradeButton);
            researchContent.Widgets.Add(upgradePanel);
        }
        
        research.Content = researchContent;
        
        _upgrades.TouchDown += (s, Desktop) => research.ShowModal(_desktop);
    }
   

    

    private void ShowUpgrades(Faction faction)
    {
        if(faction == _game.FactionManager.Player)
        {
            _upgrades.Visible = true;
            TileMapManager.OnScienceLabBuild -=ShowUpgrades;
        }  
    }

    #endregion
    private void ShowButtons()
    {
        _endTurnButton.Visible = true;
        _tradeMenuButton.Visible = true;
        TileMapManager.OnPlayerLandingBasePlaced -= ShowButtons;
    }


    private void HideButtons()
    {
        _endTurnButton.Visible = false;
        _tradeMenuButton.Visible = false;
        _upgrades.Visible = false;
        TileMapManager.OnPlayerLandingBasePlaced += ShowButtons;
        TileMapManager.OnScienceLabBuild +=ShowUpgrades;
    }
}