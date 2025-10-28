using System.Diagnostics;
using System.Linq;

namespace MemoryGame.Domain.Models;
/// <summary>
/// Represents a memory card-matching game where players flip cards to find matching pairs.
/// </summary>
/// <remarks>The <see cref="Game"/> class manages the state of the game, including the board, timer, and attempts.
/// Players can start a new game with a specified number of card pairs, flip cards to find matches, and track their
/// progress through the number of attempts and elapsed time. The game is completed when all pairs are
/// matched.</remarks>
public class Game
{
    /// <summary>
    /// 
    /// </summary>
    private readonly Board _board = new();
    private readonly Stopwatch _timer = new();

    private Card? _firstFlippedCard;

    public int Attempts { get; private set; }
    public bool IsCompleted => _board.AllMatched();
    public TimeSpan ElapsedTime => _timer.Elapsed;

    public IReadOnlyList<Card> Cards => _board.Cards;

    public void Start(int pairCount)
    {
        _board.Initialize(pairCount);
        Attempts = 0;
        _firstFlippedCard = null;
        _timer.Restart();
    }
    //Overload to allow seeding for testing purposes
    public void Start(int pairCount, int? seed)
    {
        _board.Initialize(pairCount, seed);
        Attempts = 0;
        _firstFlippedCard = null;
        _timer.Restart();
    }

    public void FlipCard(int cardId)
    {
        var card = _board.Cards.FirstOrDefault(c => c.Id == cardId); //Find card by ID, returns null if not found
        if(card == null || card.IsFaceUp || card.IsMatched)
        {
            return; //Invalid flip
        }

        card.Flip(); //Flip the card

        //Check if a card has already been flipped
        if (_firstFlippedCard == null)
        {
            _firstFlippedCard = card; //Set as first flipped card
        }
        else
        {
            Attempts++; //Increment attempts
            
            //Check for match
            if (_firstFlippedCard.Value == card.Value)
            {
                //Mark both cards as matched
                _firstFlippedCard.Match(); 
                card.Match();
            }
            else
            {
                //No match, flip both cards back down after a short delay
                _firstFlippedCard.Flip();
                card.Flip();
            }

            _firstFlippedCard = null; //Reset first flipped card

            // Check for game completion
            if (IsCompleted)
            {
                _timer.Stop(); //Stop timer if game is completed
            }

        }


    }
}

