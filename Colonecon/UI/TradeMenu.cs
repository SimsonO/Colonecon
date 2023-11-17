using System.Collections.Generic;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

public class TradeMenu
{
    private FactionManager _factionManager;
    private GamePlayUI _ui;
    private Dictionary<NPCFaction, Label[]> _tradeMenuContent;
    public HorizontalStackPanel TradeMenuPanel;
    public TradeMenu(FactionManager factionManager, GamePlayUI ui)
    {
        _factionManager = factionManager;
        _ui = ui;
        
        _tradeMenuContent = new Dictionary<NPCFaction, Label[]>();
        TradeMenuPanel = CreateTradeMenu();
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
        foreach(NPCFaction faction in _factionManager.NPCFactions)
        {
            VerticalStackPanel factionTradeMenu = CreateFactionTradeMenu(faction);
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
        Label[] tradeInfo = new Label[2];
        tradeInfo[0] = TradeAmount;
        tradeInfo[1] = TradePrice;
        _tradeMenuContent.Add(faction, tradeInfo);
        
        return factionTradeMenu;
    }

    private void BuyFactionResource(NPCFaction faction, int tradeAmount)
    {
        if(faction.AvailableTradeAmountFactionResource >= tradeAmount)
        {
            if(_factionManager.Player.BuyResources(faction.FactionResource, tradeAmount, faction.TradePrice))
            {
                faction.SellFactionResource(tradeAmount);
            }
            else
            {
                _ui.GamePlayDashboard.DisplayMessage("","You cannot afford this");
            }  
        }
        else
        {
            _ui.GamePlayDashboard.DisplayMessage("","Not enough " + faction.FactionResource + " available");
        }        
    }

    

    public void OpenTradeMenu()
    {
        UpdateTradeInformation();
        TradeMenuPanel.Visible = true;
    }

    private void UpdateTradeInformation()
    {
        foreach(NPCFaction faction in _tradeMenuContent.Keys)
        {
            _tradeMenuContent[faction][0].Text = "Available: " + faction.AvailableTradeAmountFactionResource;
            _tradeMenuContent[faction][1].Text ="Price: " + faction.TradePrice;
        }
    }
}