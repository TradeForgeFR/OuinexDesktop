﻿using OuinexDesktop.Views;
using ReactiveUI;
using OuinexDesktop.Views.Controls;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using OuinexDesktop.Models;
using DynamicData;

namespace OuinexDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            ExchangesConnector.Instances["POC-Binance"].OnInit += LoginMVVM_OnLogged;

            OpenMarketWatchWindow = ReactiveCommand.Create(() =>
            {
                var window = new ContainerWindow()
                {

                    Title = "market watch"
                };

                window.mainContainer.Children.Add(new MarketWatchWidgets()
                {
                    DataContext = MarketWatchMVVM
                });

                window.Show();
            });
           
        }

        private void LoginMVVM_OnLogged()
        {
            //close the login popup
            //InteractiveContainer.CloseDialog();

            Task.Run(async () =>
            {
                await SpotWalletsMVVM.InitAsync();
                await MarketWatchMVVM.InitStream();
            });

            SpotOrders.AddRange(ExchangesConnector.Instances["POC-Binance"].Spot.Orders);
        }

        private void CreateAControl()
        {
            var control = new Control();

        }

        public LoginViewModel LoginMVVM { get; } = new LoginViewModel();

        public MarketWatchViewModel MarketWatchMVVM { get; } = new MarketWatchViewModel();

        public SpotWallets SpotWalletsMVVM { get; } = new SpotWallets();

        public ObservableCollection<SpotOrder> SpotOrders { get; set; } = new ObservableCollection<SpotOrder>();

        public ICommand OpenMarketWatchWindow { get; }
    }
}