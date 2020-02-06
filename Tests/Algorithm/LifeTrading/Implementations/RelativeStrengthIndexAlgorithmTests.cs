using NUnit.Framework;
using QuantConnect.Algorithm.CSharp.HHLifeTrading.Enums;
using QuantConnect.Algorithm.CSharp.HHLifeTrading.Implementations;

namespace QuantConnect.Tests.Algorithm.LifeTrading.Implementations
{
    [TestFixture]
    public sealed class RelativeStrengthIndexAlgorithmTests
    {
        [TestCase(55, 25, BuySell.Buy)]
        [TestCase(35, 25, BuySell.Buy)]
        [TestCase(69, 75, BuySell.Sell)]
        [TestCase(69, 71, BuySell.Sell)]
        [TestCase(20, 90, BuySell.Sell)]
        [TestCase(40, 50, BuySell.Hold)]
        [TestCase(69, 31, BuySell.Hold)]
        [TestCase(31, 69, BuySell.Hold)]
        public void DecidePosition_Returns_Correctly_If_RSI_Move_Sufficient(decimal previousRSI, decimal currentRSI, BuySell expectedResult)
        {
            //Arrange
            int sellThreshold = 70;
            int buyThreshold = 30;
            int shortPositions = 0;
            int longPositions = 0;
            int maxPositions = 5;

            //Act
            var result = RelativeStrengthIndexAlgorithm.DecidePosition(
                previousRSI,
                currentRSI,
                sellThreshold,
                buyThreshold,
                shortPositions,
                longPositions,
                maxPositions
            );

            //Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestCase(55, 25, BuySell.Hold)]
        [TestCase(35, 25, BuySell.Hold)]
        [TestCase(69, 75, BuySell.Hold)]
        [TestCase(69, 71, BuySell.Hold)]
        [TestCase(20, 90, BuySell.Hold)]
        [TestCase(40, 50, BuySell.Hold)]
        [TestCase(69, 31, BuySell.Hold)]
        [TestCase(69, 31, BuySell.Hold)]
        public void DecidePosition_Does_Not_BuyOrSell_If_Limit_Reached(decimal previousRSI, decimal currentRSI, BuySell expectedResult)
        {
            //Arrange
            int sellThreshold = 70;
            int buyThreshold = 30;
            int shortPositions = 5;
            int longPositions = 5;
            int maxPositions = 5;

            //Act
            var result = RelativeStrengthIndexAlgorithm.DecidePosition(
                previousRSI,
                currentRSI,
                sellThreshold,
                buyThreshold,
                shortPositions,
                longPositions,
                maxPositions
            );

            //Assert
            Assert.AreEqual(expectedResult, result);
        }
    }
}
