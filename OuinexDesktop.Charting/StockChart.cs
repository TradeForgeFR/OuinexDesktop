﻿using CNergyTrader.Indicator;
using CNergyTrader.Indicator.Indicators;
using ReactiveUI;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;

namespace OuinexDesktop.Charting
{
    public enum ChartType
    {
        Candlestick,
        OHLC,
        Line
    } 

    public class StockChart : ReactiveObject
    {
        #region private fields
        private ChartType _chartType = ChartType.OHLC;
        private AvaPlot _plot;
        private Crosshair _crossHair;
        private OHLC[] _price;
        private FinancePlot _candlesPlot, _ohlcsPlot;



        #endregion

        #region Constructor
        public StockChart()
        {
            // init
            setupBases();
        }
        #endregion

        public ChartType SelectedChartType
        {
            get => _chartType;
            set
            {
                this.RaiseAndSetIfChanged(ref _chartType, value);
                
                switch (_chartType)
                {
                    case ChartType.Candlestick:
                        _candlesPlot.IsVisible = true;
                        _ohlcsPlot.IsVisible = false;
                        break;
                    case ChartType.OHLC:
                        _candlesPlot.IsVisible = false;
                        _ohlcsPlot.IsVisible = true;
                        break;
                }

                MainPlotArea.Refresh();
            }
        }

        public AvaPlot MainPlotArea
        {
            get => _plot;
            private set => _plot = value;
        }

        private void setupBases()
        {
            MainPlotArea = new AvaPlot();

            _crossHair = MainPlotArea.Plot.AddCrosshair(0, 0);
            _crossHair.IgnoreAxisAuto = true;
            _crossHair.LineStyle = LineStyle.Solid;
            _crossHair.LineWidth = 1;
            _crossHair.Color = System.Drawing.Color.Black;

            MainPlotArea.PointerMoved += (o, e) =>
            {
                (double coordinateX, double coordinateY) = MainPlotArea.GetMouseCoordinates();

                _crossHair.X = coordinateX;
                _crossHair.Y = coordinateY;                 

                MainPlotArea.Refresh();
            };

            GenerateTestDatas();
        }

        public void GenerateTestDatas()
        {
            _price = DataGen.RandomStockPrices(new Random(), 500);

            _candlesPlot = MainPlotArea.Plot.AddCandlesticks(_price);
            _candlesPlot.IsVisible = false;
            _candlesPlot.ColorUp = System.Drawing.Color.Black;

            _ohlcsPlot = MainPlotArea.Plot.AddOHLCs(_price);

            MainPlotArea.Refresh();

            AddDatasTestCommand = ReactiveCommand.Create(() =>
            {
                _price = DataGen.RandomStockPrices(new Random(), 1500);

                _candlesPlot.Clear();
                _ohlcsPlot.Clear();

                _candlesPlot.AddRange(_price);
                _ohlcsPlot.AddRange(_price);
                
                MainPlotArea.Plot.AxisAuto();
                MainPlotArea.Refresh();

                AddIndicator();
            });
        }

        public void AddIndicator()
        {
            var Indicator = new Ichimoku();

            var manager = new OnPriceIndicatorItem(Indicator, MainPlotArea);

            Indicator.Calculate(_price.Count(), null, _price.Select(x => x.Open).ToArray(), _price.Select(x => x.High).ToArray(), _price.Select(x => x.Low).ToArray(), _price.Select(x => x.Close).ToArray(), null);

            MainPlotArea.Refresh();
        }

        public IEnumerable<ChartType> ChartTypes
        {
            get => Enum.GetValues(typeof(ChartType)).Cast<ChartType>();
        }

        public IReactiveCommand AddDatasTestCommand { get; private set; }
    }

    public class OnPriceIndicatorItem
    {
        private IndicatorBase _indicator;
        private AvaPlot _priceArea;

        public OnPriceIndicatorItem(IndicatorBase indicator, AvaPlot priceArea) 
        {
            _indicator = indicator;
            _priceArea = priceArea;

            _indicator.Init();
            setupSeries();
        }

        private void setupSeries()
        {
            foreach(var serie in _indicator.Series)
            {
                var line = _priceArea.Plot.AddScatterLines(null, null, serie.DefaultColor);
                line.OnNaN = ScatterPlot.NanBehavior.Gap;
                line.StepDisplayRight = true;
           
                _indicator.OnCalculated += () =>
                {
                    line.Update(serie.Select(x => (double)serie.IndexOf(x)).ToArray(), serie.ToArray());
                };
            }
        }
    }
}