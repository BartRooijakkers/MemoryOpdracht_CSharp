using System.Linq;
using System.Threading;
using MemoryGame.Domain.Models;
using Xunit;

namespace MemoryGame.Tests
{
    public class GameTests
    {
        [Fact]
        public void Start_ShouldInitializeBoard_AndResetStats()
        {
            var game = new Game();
            game.Start(3); // 3 pairs = 6 cards

            Assert.Equal(6, game.Cards.Count); // Check if there are exactly 6 cards
            Assert.Equal(0, game.Attempts); // Attempts should be reset to 0
            Assert.False(game.IsCompleted);// Game should not be completed at start
        }

        [Fact]
        public void FlipCard_FirstFlip_ShouldOnlyTurnOneCardFaceUp()
        {
            var game = new Game();
            game.Start(2); //2 pairs = 4 cards

            game.FlipCard(0); //Flip first card

            Assert.True(game.Cards[0].IsFaceUp); // First card should be face up
            Assert.Equal(0, game.Attempts);// Attempts should still be 0
        }

        [Fact]
        public void FlipCard_SecondFlip_MatchingPair_ShouldMarkBothAsMatched()
        {
            var game = new Game();
            game.Start(1);// 1 pair (ids 0 and 1)

            game.FlipCard(0); //Flip first card
            game.FlipCard(1); //Flip matching card

            Assert.All(game.Cards, c => Assert.True(c.IsMatched)); // Both cards should be marked as matched
            Assert.Equal(1, game.Attempts); // Attempts should be incremented
            Assert.True(game.IsCompleted);// Game should be completed
        }

        [Fact]
        public void FlipCard_SecondFlip_NotMatching_ShouldFlipBothBackDown()
        {
            var game = new Game();
            game.Start(2); // 2 pairs

            // Find two cards that do not match
            var first = game.Cards[0]; // Get first card
            var second = game.Cards.First(c => c.Value != first.Value); // Get a card with a different value

            game.FlipCard(first.Id); //Flip first card
            game.FlipCard(second.Id);// Flip non-matching card

            Assert.False(first.IsFaceUp); // Both cards should be face down again
            Assert.False(second.IsFaceUp);//
            Assert.Equal(1, game.Attempts); // Attempts should be incremented
        }

        [Fact]
        public void Game_ShouldStopTimer_WhenCompleted()
        {
            var game = new Game();
            game.Start(1); // 1 pair (ids 0 and 1)

            game.FlipCard(0); //Flip first card
            Thread.Sleep(100); //Wait a bit to ensure time has passed
            game.FlipCard(1); //Flip matching card

            var elapsedAfter = game.ElapsedTime; //Capture elapsed time after completion
            Thread.Sleep(100);// Wait more to see if time changes
            Assert.Equal(elapsedAfter, game.ElapsedTime); // Elapsed time should not change after completion
            Assert.True(game.IsCompleted); // Game should be completed
        }
    }
}
