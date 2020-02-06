using System;
using NUnit.Framework;
using QuantConnect.Algorithm.CSharp.HHLifeTrading.Implementations;
using QuantConnect.Algorithm.CSharp.HHLifeTrading.Interfaces;
using QuantConnect.Indicators;

namespace QuantConnect.Tests.Algorithm.LifeTrading.Implementations
{
    [TestFixture]
    public sealed class RelativeStrengthIndicatorTests
    {
        private IRelativeStrengthIndicator _sut;
        private int _periodLength = 5;
        private const decimal Tolerance = 0.0000000001M;


        [SetUp]
        public void Setup()
        {
            _sut = new RelativeStrengthIndicator(_periodLength);
        }

        [Test]
        public void Ctor_Sets_Properties()
        {
            //Assert
            Assert.AreEqual(_periodLength, _sut.PeriodLength);
            Assert.IsFalse(_sut.IsReady);
        }

        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-1)]
        public void Ctor_Throws_With_Invalid_PeriodLength(int invalidPeriodLength)
        {
            Assert.Throws<ArgumentException>(() => _sut = new RelativeStrengthIndicator(invalidPeriodLength));
        }

        [Test]
        public void ComputeNextValue_Computes_Correctly()
        {
            //Arrange
            var closePrices = new[]
            {
                1243.91, 1247.08, 1254.6, 1258.84, 1256.77, 1257.54, 1258.51, 1259.78, 1257.88, 1257.64, 1271.87
            };

            var expectedRelativeStrengthIndex = new[]
            {
                -1, -1, -1, -1, -1, 88.3511536297132, 89.0952192809169, 90.1272841088624, 76.5743519608969,
                74.7983771213703, 90.7310371810282
            };
            var expectedIsReady = new[] { false, false, false, false, false, true, true, true, true, true, true };

            //Act & Assert
            for (var i = 0; i < closePrices.Length; i++)
            {
                var currentPrice = closePrices[i];
                var isReady = _sut.Update(new IndicatorDataPoint(DateTime.Now.AddDays(i), (decimal)currentPrice));
                Assert.AreEqual(expectedIsReady[i], isReady);

                var priceDifference = Math.Abs((decimal) expectedRelativeStrengthIndex[i] - _sut.Current.Price);
                Assert.IsTrue(priceDifference < Tolerance);
            }
        }
    }
}
