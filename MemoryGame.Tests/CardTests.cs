using MemoryGame.Domain.Models;

namespace MemoryGame.Tests
{
    public class CardTests
    {
        [Fact]
        public void Flip_Twice_Should_Return_To_FaceDown()
        {
            var card = new Card(1, "A");

            card.Flip();
            card.Flip();

            Assert.False(card.IsFaceUp);
        }

        [Fact]
        public void Match_Should_Set_IsMatched_And_IsFaceUp_True()
        {
            var card = new Card(2, "B");

            card.Match();

            Assert.True(card.IsMatched);
            Assert.True(card.IsFaceUp);
        }

        [Fact]
        public void Flip_Should_Not_Change_State_When_Matched()
        {
            var card = new Card(3, "C");
            card.Match();

            card.Flip();

            Assert.True(card.IsFaceUp);
            Assert.True(card.IsMatched);
        }
    }
}