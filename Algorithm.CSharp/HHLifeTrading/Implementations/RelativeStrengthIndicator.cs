using System;
using QuantConnect.Algorithm.CSharp.HHLifeTrading.Interfaces;
using QuantConnect.Indicators;

namespace QuantConnect.Algorithm.CSharp.HHLifeTrading.Implementations
{
    public sealed class RelativeStrengthIndicator : Indicator, IRelativeStrengthIndicator
    {
        private readonly int _periodLength;
        private decimal _totalGains;
        private decimal _totalLosses;
        private decimal _averageGains;
        private decimal _averageLosses;
        private decimal _previousClose;

        public RelativeStrengthIndicator(int periodLength) : base("Relative Strength Indicator")
        {
            if (periodLength <= 1) throw new ArgumentException("Period Length must be greater than 1");
            _periodLength = periodLength;
        }

        public int PeriodLength
        {
            get { return _periodLength; }
        }
        public override bool IsReady => Samples > _periodLength;

        private static decimal _defaultRelativeStrengthIndicator = -1;
        protected override decimal ComputeNextValue(IndicatorDataPoint input)
        {
            if (Samples == 1)
            {
                _previousClose = input.Price;
                return _defaultRelativeStrengthIndicator;
            }

            decimal newGain = 0;
            decimal newLoss = 0;
            var priceMove = input.Price - _previousClose;

            if (priceMove > 0) newGain = priceMove;
            else newLoss = -priceMove;

            _totalGains += newGain;
            _totalLosses += newLoss;

            if (Samples - 1 <= _periodLength)
            {
                _averageGains = _totalGains / _periodLength;
                _averageLosses = _totalLosses / _periodLength;

            }
            else
            {
                _averageGains = (_averageGains * (_periodLength - 1) + newGain) / _periodLength;
                _averageLosses = (_averageLosses * (_periodLength - 1) + newLoss) / _periodLength;
            }

            var relativeStrengthIndex = _defaultRelativeStrengthIndicator;
            if (Samples > _periodLength)
            {
                decimal relativeStrength = _averageLosses == 0 ? _averageGains : _averageGains / _averageLosses;
                relativeStrengthIndex = 100 - 100 / (1 + relativeStrength);
            }

            _previousClose = input.Price;
            return relativeStrengthIndex;
        }



    }
}
