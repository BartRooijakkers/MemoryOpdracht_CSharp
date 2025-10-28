using MemoryGame.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Tests
{
    public class ScoreCalculatorTests
    {
        //Test various scenarios for the ScoreCalculator.Calculate method as also seen on the github
        [Theory]
        [InlineData(4, 10, 2, 800)]   // (16/20)*1000 = 800
        [InlineData(10, 20, 5, 1000)] // (100/100)*1000 = 1000
        [InlineData(4, 20, 2, 400)]   // (16/40)*1000 = 400
        [InlineData(4, 10, 3, 533)]   // 533.33 -> 533
        public void Calculate_Examples_ReturnExpected(int cards, int seconds, int attempts, int expected)
        {
            Assert.Equal(expected, ScoreCalculator.Calculate(cards, seconds, attempts));// Verify that the calculated score matches the expected value
        }

        //Test various invalid input scenarios
        [Theory]
        [InlineData(0, 10, 1)]
        [InlineData(4, 0, 1)]
        [InlineData(4, 10, 0)]
        public void Calculate_InvalidInputs_ReturnsZero(int cards, int seconds, int attempts)
        {
            Assert.Equal(0, ScoreCalculator.Calculate(cards, seconds, attempts));// Invalid inputs should return a score of 0
        }
    }
}
