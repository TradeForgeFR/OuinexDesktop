﻿using DynamicData;
using DynamicData.Aggregation;
using DynamicData.Binding;
using OuinexDesktop.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OuinexDesktop.ViewModels
{
    public class SpotWalletItem : ViewModelBase
    {
        private string _token;
        private decimal _amount, _usdValue, _profit, _lockedValue, _averageBuyingPrice, _currentPrice;
        private Ticker _ticker;

        public SpotWalletItem(string token)
        {
            Token = token;
        }

        public string Token
        {
            get =>  _token;
            private set => _token = value;
        }

        public decimal Amount
        {
            get { return _amount; }
            set => this.RaiseAndSetIfChanged(ref _amount, value, nameof(Amount));
        }

        public decimal UsdValue
        {
            get => _usdValue;
            set=> this.RaiseAndSetIfChanged(ref _usdValue, value, nameof(UsdValue));
        }

        public decimal Profit
        {
            get=> _profit;
            set => this.RaiseAndSetIfChanged(ref _profit, value, nameof(Profit));
        }

        public decimal Locked
        {
            get => _lockedValue;
            set => this.RaiseAndSetIfChanged(ref _lockedValue, value, nameof(Locked));
        }

        public decimal AverageBuyingPrice
        {
            get => _averageBuyingPrice;
            set => this.RaiseAndSetIfChanged(ref _averageBuyingPrice, value, nameof(AverageBuyingPrice));
        }

        public decimal CurrentPrice
        {
            get => _currentPrice;
            set => this.RaiseAndSetIfChanged(ref _currentPrice, value, nameof(CurrentPrice));
        }

        public Ticker Ticker
        {
            get => _ticker;
            set
            {
                this.RaiseAndSetIfChanged(ref _ticker, value, nameof(Ticker));
                
                if(_ticker!= null)
                {
                    _ticker.OnTick += _ticker_OnTick;
                }
            }
        }

        private void _ticker_OnTick(Ticker ticker)
        {
            CurrentPrice = ticker.AskPrice;

            Profit = (CurrentPrice - AverageBuyingPrice) * Amount;

            UsdValue = Amount * ticker.AskPrice;
        }
    }

    public class SpotWallets : ViewModelBase
    {
        private string _totalValue, _totalPnL = string.Empty;

        public SpotWallets()
        {
            Wallets.Add(new SpotWalletItem("Bitcoin")
            {
                Amount = (decimal)0.00877428,
                AverageBuyingPrice=(decimal)18892.23
            });

            Wallets.Add(new SpotWalletItem("Ouinex Token")
            {
                Amount = (decimal)2000652,
                AverageBuyingPrice = (decimal)0.025,
                CurrentPrice = (decimal)0.54,
                UsdValue = (decimal)(2000652 * 0.54),
                Profit = (decimal)((0.54 - 0.025) * 2000652)
            });

            Wallets.Add(new SpotWalletItem("Ethereum")
            {
                Amount = (decimal)160.762,
                AverageBuyingPrice = (decimal)1834.65,
            });
        }

        public async Task InitAsync()
        {
            var btc = ExchangesConnector.Instances.First().Value.Spot.Symbols.FirstOrDefault(x => x.BaseCurrency == "BTC" && x.QuoteCurrency == "USDT");
            var eth = ExchangesConnector.Instances.First().Value.Spot.Symbols.FirstOrDefault(x => x.BaseCurrency == "ETH" && x.QuoteCurrency == "USDT");

            var tikcerBTC = await ExchangesConnector.Instances.First().Value.Spot.GetTickerAsync(btc);
            var tikcerETH = await ExchangesConnector.Instances.First().Value.Spot.GetTickerAsync(eth);

            this.Wallets.First().Ticker = tikcerBTC;
            this.Wallets.First(x => x.Token == "Ethereum").Ticker = tikcerETH;

            TotalValue = string.Format(" ≃ $ {0}", this.Wallets.Sum(x => x.UsdValue));
        }

        public string TotalValue
        {
            get => _totalValue;
            set => this.RaiseAndSetIfChanged(ref _totalValue, value, nameof(TotalValue));
        }

        public ObservableCollection<SpotWalletItem> Wallets { get; set; } = new ObservableCollection<SpotWalletItem>();
    }
}