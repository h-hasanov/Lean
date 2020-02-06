using System;
using QuantConnect.Algorithm.CSharp.HHLifeTrading.Enums;
using QuantConnect.Algorithm.CSharp.HHLifeTrading.Interfaces;
using QuantConnect.Data;
using QuantConnect.Securities;

namespace QuantConnect.Algorithm.CSharp.HHLifeTrading.Implementations
{
    public sealed class RelativeStrengthIndexAlgorithm : QCAlgorithm
    {
        private readonly DateTime _startDate = new DateTime(1998, 01, 02);
        private readonly DateTime _endDate = new DateTime(2018, 07, 03);
        private readonly Resolution _resolution = Resolution.Daily;
        private Security _security;
        private const int WarmUpDays = 5;
        private const string Ticker = "AAPL";
        private IRelativeStrengthIndicator _relativeStrengthIndicator;
        private const int BuyThreshold = 30;
        private const int SellThreshold = 70;
        private const int MaxPositions = 10;
        private int _shortPositions;
        private int _longPositions;
        private decimal _previousRelativeStrengthIndicator = -1;

        public override void Initialize()
        {
            SetStartDate(_startDate);
            SetEndDate(_endDate);
            _security = AddEquity(Ticker, _resolution);
            _relativeStrengthIndicator = CreateRelativeStrengthIndicator(_security.Symbol, _resolution);
        }

        public override void OnData(Slice slice)
        {
            if (_relativeStrengthIndicator == null || !_relativeStrengthIndicator.IsReady) return;
            var currentRelativeStrengthIndicator = _relativeStrengthIndicator.Current.Price;

            if (_previousRelativeStrengthIndicator >= 0)
            {
                var position = DecidePosition(
                    _previousRelativeStrengthIndicator,
                    currentRelativeStrengthIndicator,
                    SellThreshold,
                    BuyThreshold,
                    _shortPositions,
                    _longPositions,
                    MaxPositions
                );
                if (position == BuySell.Sell)
                {
                    Sell(_security.Symbol, 1);
                    _shortPositions++;
                }

                if (position == BuySell.Buy)
                {
                    Buy(_security.Symbol, 1);
                    _longPositions++;
                }
            }
            _previousRelativeStrengthIndicator = currentRelativeStrengthIndicator;
        }

        public static BuySell DecidePosition(decimal previousRelativeStrengthIndicator,
            decimal currentRelativeStrengthIndicator, 
            int sellThreshold,
            int buyThreshold,
            int shortPositions,
            int longPositions,
            int maxPositions)
        {
            if (previousRelativeStrengthIndicator < sellThreshold &&
                currentRelativeStrengthIndicator > sellThreshold && 
                shortPositions < maxPositions)
            {
                return BuySell.Sell;
            }

            if (previousRelativeStrengthIndicator > buyThreshold &&
                currentRelativeStrengthIndicator < buyThreshold && 
                longPositions < maxPositions)
            {
                return BuySell.Buy;
            }

            return BuySell.Hold;
        }

        private IRelativeStrengthIndicator CreateRelativeStrengthIndicator(Symbol symbol, Resolution resolution)
        {
            var relativeStrengthIndicator = new RelativeStrengthIndicator(WarmUpDays);
            RegisterIndicator(symbol, relativeStrengthIndicator, resolution, c => c.Value);
            WarmUpIndicator(symbol, relativeStrengthIndicator, resolution);

            return relativeStrengthIndicator;
        }
    }
}