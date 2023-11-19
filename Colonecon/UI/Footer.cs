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
    private TurnManager _turnManager;
    public Building SelectedBuilding {get; private set;}
    private List<Building> _buildOptions;
    private List<Building> _startingBuilding;
    private HorizontalStackPanel _buildOptionPanel;
    private Dictionary<Building, Button> _buildingButtons = new Dictionary<Building, Button>(); 
    public Footer(ColoneconGame game, GamePlayUI ui, TurnManager turnManager)
    {
        _game = game;
        _buildOptions = game.BuildOptionLoader.BuildOptions;
        
        _startingBuilding = new List<Building>
        {
            game.BuildOptionLoader.StartingBase
        };
        SelectedBuilding = game.BuildOptionLoader.StartingBase;

        _ui = ui;
        _turnManager = turnManager;

        TileMapManager.OnPlayerLandingBasePlaced += FillBuildingSection;
        TileMapManager.OnNotEnoughResources += InformAboutMissingRessources;
    }
    public HorizontalStackPanel CreateFooter()
    {
        HorizontalStackPanel footer = new HorizontalStackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Bottom
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
        openTradeMenu.TouchDown += (s,a) => _ui.TradeMenu.OpenTradeMenu();

        _buildOptionPanel = new HorizontalStackPanel()
        {
            Spacing = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            Background = new SolidBrush(GlobalColorScheme.BackgroundColor)
        };
        FillBuildingOptions(_startingBuilding);
        footer.Widgets.Add(_buildOptionPanel);
        footer.Widgets.Add(openTradeMenu);
        footer.Widgets.Add(endTurnButton);
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
        }
    }

    private void FillBuildingSection()
    {
        TileMapManager.OnPlayerLandingBasePlaced -= FillBuildingSection;
        TileMapManager.OnBuildingPlaced += ClearBuildingSelection;
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
            _ui.GamePlayDashboard.DisplayMessage("Not enough Ressources", message);
        }
    }
}