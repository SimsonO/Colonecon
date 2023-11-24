using System;
using System.Collections.Generic;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;

public class TradeMenu
{
    private FactionManager _factionManager;
    private GamePlayUI _ui;
    private Dictionary<NPCFaction, Label[]> _tradeMenuContent;
    private Label[] _offerContent;
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
            Background = new SolidBrush(GlobalColorScheme.BackgroundColor),
            Spacing = 32
        };
        Label rowCaptions = new Label
        {
            Text = "/n Resource /n Available /n Price "
        };
        tradeMenu.Widgets.Add(rowCaptions);
        foreach(NPCFaction faction in _factionManager.NPCFactions)
        {
            VerticalStackPanel factionTradeMenu = CreateFactionTradeMenu(faction);
            tradeMenu.Widgets.Add(factionTradeMenu);
        }

        VerticalStackPanel buyFromHomeMenu = CreateHomeTradeMenu();
        tradeMenu.Widgets.Add(buyFromHomeMenu);
        VerticalStackPanel sellMenu = CreateSellMenu();
        tradeMenu.Widgets.Add(sellMenu);

        Button closeMenu = new Button
        {
            Content = new Label
            {
                Text = "X"
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
          Text = faction.AvailableTradeAmountFactionResource.ToString()
        };
        Label TradePrice = new Label
        {
          Text = faction.TradePrice.ToString()
        };
        Button buy1factionResource = new Button
        {
            Content = new Label
            {
                Text = "+1"
            }
        };
        buy1factionResource.TouchDown += (s, Faction) => BuyFactionResource(faction, 1);
        Button buy5factionResource = new Button
        {
            Content = new Label
            {
                Text = "+5"
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
                UpdateTradeInformation();
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

    private VerticalStackPanel CreateHomeTradeMenu()
    {
        Player player = _factionManager.Player;
        VerticalStackPanel HomeTradeMenu = new VerticalStackPanel
        {

        };
        Label homeName = new Label
        {
          Text = player.Name  
        };
        Label homeResource = new Label
        {
          Text = player.FactionResource.ToString()  
        };
        Label TradePrice = new Label
        {
          Text = "inf /n" + player.FactionResourcePrice
        };
        Button buy1factionResource = new Button
        {
            Content = new Label
            {
                Text = "+1"
            }
        };
        buy1factionResource.TouchDown += (s, a) => BuyHomeResource(1);
        Button buy5factionResource = new Button
        {
            Content = new Label
            {
                Text = "+5"
            }
        };
        buy5factionResource.TouchDown += (s, a) => BuyHomeResource(5);
        HomeTradeMenu.Widgets.Add(homeName);
        HomeTradeMenu.Widgets.Add(homeResource);
        HomeTradeMenu.Widgets.Add(TradePrice);
        HomeTradeMenu.Widgets.Add(buy1factionResource);
        HomeTradeMenu.Widgets.Add(buy5factionResource);

        return HomeTradeMenu;
    }

    private void BuyHomeResource(int amount)
    {
        if(!_factionManager.Player.TradeFromHome(amount))
        {
            _ui.GamePlayDashboard.DisplayMessage("","You cannot afford this");
        }
        UpdateTradeInformation();
    }

    private VerticalStackPanel CreateSellMenu()
    {
        Player player = _factionManager.Player;
        VerticalStackPanel SellMenu = new VerticalStackPanel
        {

        };
        Label title = new Label
        {
          Text = "Offer for sale /n" + player.FactionResource 
        };
        Label available = new Label
        {
             Text = player.AvailableTradeAmountFactionResource.ToString()
        };
        Label tradePrice = new Label
        {
          Text = player.FactionResourcePrice.ToString()
        };
        HorizontalStackPanel buysell1 = new HorizontalStackPanel()
        {
            Spacing = 32
        };
        HorizontalStackPanel buysell5 = new HorizontalStackPanel()
        {
            Spacing = 32
        };
        Button offer1factionResource = new Button
        {
            Content = new Label
            {
                Text = "+1"
            }
        };
        offer1factionResource.TouchDown += (s, a) => OfferHomeResource(1);
        buysell1.Widgets.Add(offer1factionResource);
        Button offer5factionResource = new Button
        {
            Content = new Label
            {
                Text = "+5"
            }
        };
        offer5factionResource.TouchDown += (s, a) => OfferHomeResource(5);
        buysell5.Widgets.Add(offer5factionResource);
        Button reduceoffer1factionResource = new Button
        {
            Content = new Label
            {
                Text = "-1"
            }
        };
        reduceoffer1factionResource.TouchDown += (s, a) => ReduceOfferResource(1);
        buysell1.Widgets.Add(reduceoffer1factionResource);
        Button reduceoffer5factionResource = new Button
        {
            Content = new Label
            {
                Text = "-5"
            }
        };
        reduceoffer5factionResource.TouchDown += (s, a) => ReduceOfferResource(5);
        buysell5.Widgets.Add(reduceoffer5factionResource);
        SellMenu.Widgets.Add(title);
        SellMenu.Widgets.Add(available);
        SellMenu.Widgets.Add(tradePrice);
        SellMenu.Widgets.Add(buysell1);
        SellMenu.Widgets.Add(buysell5);

        _offerContent = new Label[2]
        {
            available, tradePrice
        };

        return SellMenu;
    }

    private void OfferHomeResource(int amount)
    {
        if(!_factionManager.Player.IncreaseAvailableTradeAmount(amount))
        {
            _ui.GamePlayDashboard.DisplayMessage("","Not enough Resources");
        }
        UpdateTradeInformation();
    }

    private void ReduceOfferResource(int amount)
    {
        if(!_factionManager.Player.DecreaseAvailableTradeAmount(amount))
        {
            _ui.GamePlayDashboard.DisplayMessage("","You have not offered this much");
        }
        UpdateTradeInformation();
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
            _tradeMenuContent[faction][0].Text = faction.AvailableTradeAmountFactionResource.ToString();
            _tradeMenuContent[faction][1].Text = faction.TradePrice.ToString();
        }
        _offerContent[0].Text = _factionManager.Player.AvailableTradeAmountFactionResource.ToString();
        _offerContent[1].Text = _factionManager.Player.TradePrice.ToString();
    }
}