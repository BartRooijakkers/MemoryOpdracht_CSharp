using System.Diagnostics;
using System.Linq;
using MemoryGame.Domain.Interfaces;

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
    /// <summary>
    /// Starts a new game session with the specified number of card pairs.
    /// </summary>
    /// <remarks>This initializes the game board, resets the attempt counter, clears any previously
    /// flipped cards,  and restarts the game timer. <paramref name="pairCount"/> has to be greater than zero to
    /// avoid invalid game states.</remarks>
    /// <param name="pairCount">The number of card pairs to include in the game. Must be a positive integer.</param>
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
    /// <summary>
    /// Flips a card on the game board, showing its value if it is face down and not already matched.
    /// </summary>
    /// <remarks>If the  card is already face up, matched, or does not exist, the method performs no
    /// action. When a card is flipped, it is checked against the previously flipped card (if there is one) to determine if they
    /// match: If the cards match, both are marked as matched. If the cards do not match, both are flipped back face
    /// down after a short delay. The method also increments the attempt counter and stops the game timer if all cards
    /// are matched, completing the game.</remarks>
    /// <param name="cardId">The unique identifier of the card to flip.</param>
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
                //DELAY INBOUWEN? 
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
    /// <summary>
    /// Calculates the score for the current game based on the number of cards, elapsed time, and attempts.
    /// </summary>
    /// <remarks>The score is only calculated for completed games. If the game is not completed, the method
    /// returns 0. The calculation takes total number of cards on the board, the elapsed time in
    /// seconds,  and the number of attempts. Making sure that elapsedTime and attempts are not zero. The score is calculated using: <see cref="ScoreCalculator"/>. </remarks>
    /// <returns>The calculated score as an integer. Returns 0 if the game is not completed.</returns>
    public int CalculateScore()
    {
        if(!IsCompleted)
        {
            return 0; //Score only calculable on completed games
        }

        int cards = _board.Cards.Count; //Total number of cards currently on board
        int seconds = (int)Math.Max(1,ElapsedTime.TotalSeconds); //Avoid division by zero, by forcing the minimum to 1 second
        int attempts = Math.Max(1, Attempts); //Avoid division by zero, by forcing the minimum to 1 attempt
        return ScoreCalculator.Calculate(cards, seconds, attempts); //Calculate score using the ScoreCalculator 

    }
    /// <summary>
    /// Saves the player's high score to the specified high score repository.
    /// </summary>
    /// <remarks>Only completed games can save high scores.The score is calculated using the <see cref="ScoreCalculator"/>.</remarks>
    /// <param name="repo">The repository where the high score will be saved. Cannot be <see langword="null"/>.</param>
    /// <param name="playerName">The name of the player with the high score.</param>
    /// <returns>A tuple containing the following: <list type="bullet"> <item> <description><see cref="bool"/>: <see
    /// langword="true"/> if the high score was added in the repositorY. otherwise, <see
    /// langword="false"/>.</description> </item> <item> <description><see cref="int"/>: The rank of the high score in
    /// the repository or -1 if the high score was not added.</description> </item> <item> <description><see
    /// cref="HighScoreEntry"/>: The high score enty that was created regardless of whether it was added to the
    /// repository.</description> </item> </list></returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="repo"/> is <see langword="null"/>.</exception>
    public (bool added, int rank, HighScoreEntry entry) SaveHighScore(IHighScoreRepository repo, string playerName){
        if(repo is null){
            throw new ArgumentNullException(nameof(repo)); //Repository is required
        }


        int cards = _board.Cards.Count; //Total number of cards currently on board
        int seconds = (int)Math.Max(1, ElapsedTime.TotalSeconds); //Avoid division by zero, by forcing the minimum to 1 second
        int attempts = Math.Max(1, Attempts); //Avoid division by zero, by forcing the minimum to 1 attempt


        if (!IsCompleted)
        { //Only completed games can save high scores
            return (false,-1,new HighScoreEntry(playerName,0,_board.Cards.Count,Attempts,(int)Math.Max(1, ElapsedTime.TotalSeconds),DateTime.Now)); //Cannot save high score for incomplete game
        }

        //Calculate score
        int points = ScoreCalculator.Calculate(cards,seconds,attempts);

        var entry = new HighScoreEntry(playerName, points, cards, attempts, seconds, DateTime.Now); //Create high score entry


        var (added, rank) = repo.AddOrUpdateTop10(entry); //Try to add/update the high score in the repository
        return (added, rank, entry); //Return result

    }

}

