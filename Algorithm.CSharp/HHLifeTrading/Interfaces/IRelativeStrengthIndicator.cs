using QuantConnect.Indicators;

namespace QuantConnect.Algorithm.CSharp.HHLifeTrading.Interfaces
{
    public interface IRelativeStrengthIndicator : IIndicator
    {
        int PeriodLength { get; }
    }
}
