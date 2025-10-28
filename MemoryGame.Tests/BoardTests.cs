using System.Linq;
using MemoryGame.Domain.Models;
using Xunit;

namespace MemoryGame.Tests
{
    public class BoardTests
    {
        [Fact]
        public void Initialize_With8Pairs_ShouldCreate16Cards()
        {
            var board = new Board();

            board.Initialize(pairCount: 8); // 8 pairs = 16 cards

            Assert.Equal(16, board.Cards.Count); // Check if there are exactly 16 cards
        }

        [Fact]
        public void Initialize_ShouldCreateExactlyTwoOfEachValue()
        {
            var board = new Board();
            board.Initialize(pairCount: 4); // values: 0,1,2,3 should be present twice

            var groups = board.Cards.GroupBy(c => c.Value); // Group by card value
            Assert.All(groups, g => Assert.Equal(2, g.Count())); // Each group should have exactly 2 cards
        }

        [Fact]
        public void Initialize_WithSameSeed_ProducesSameShuffle()
        {
            var b1 = new Board();
            var b2 = new Board();

            b1.Initialize(pairCount: 5, seed: 42); // same seed
            b2.Initialize(pairCount: 5, seed: 42); // same seed

            var seq1 = b1.Cards.Select(c => c.Value).ToArray(); //Should be in the same order
            var seq2 = b2.Cards.Select(c => c.Value).ToArray(); //Should be in the same order

            Assert.Equal(seq1, seq2); // Check if both sequences are equal
        }

        [Fact]
        public void AllMatched_ShouldReturnTrue_WhenEveryCardIsMatched()
        {
            var board = new Board();
            board.Initialize(pairCount: 3); // 3 pairs = 6 cards

            foreach (var c in board.Cards)// Loop through all cards and mark them as matched
            {
                c.Match();
            } 
               

            Assert.True(board.AllMatched());// Should return true since all cards have been flagges as matched
        }

        [Fact]
        public void AllMatched_ShouldReturnFalse_WhenAnyCardNotMatched()
        {
            var board = new Board();
            board.Initialize(pairCount: 3); // 3 pairs = 6 cards

            // Only
            foreach (var c in board.Cards.Take(3)) // Match only first 3 cards using LINQ
            {
                c.Match();
            }

            Assert.False(board.AllMatched()); // Should return false since not all cards are matched
        }
    }
}