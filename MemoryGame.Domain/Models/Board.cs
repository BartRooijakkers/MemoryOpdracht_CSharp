using System.Linq;

namespace MemoryGame.Domain.Models;

/// <summary>
/// Represents a game board consisting of a collection of pairs of cards. Possiblity to initialize the board with
/// card pairs and check if all cards are matched.
/// </summary>
/// <remarks>The board maintains an internal collection of cards, which can be accessed in a read-only manner 
/// through the <see cref="Cards"/> property. The <see cref="Initialize(int, int?)"/> method is used to  populate the
/// board with a specified number of card pairs, optionally using a seed for recreatable shuffling.</remarks>
public class Board
{
    public IReadOnlyList<Card> Cards => _cards; //Readonly exposure of cards
    private readonly List<Card> _cards = new(); //Internal list of cards

    //If Board is not empty and all cards are matched
    /// <summary>
    /// Determines whether all cards in the board are matched.
    /// </summary>
    /// <remarks>This method returns <see langword="true"/> if the board contains at least one card  and all
    /// cards have been matched. Otherwise, it returns <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if the board is not empty and all cards are matched;  otherwise, <see langword="false"/>.</returns>
    public bool AllMatched() => _cards.Count > 0 && _cards.All(c => c.IsMatched);

    public Board()
    {

    }
    /// <summary>
    /// Initializes the collection of cards with the given number of pairs and shuffles them.
    /// </summary>
    /// <remarks>This method generates a collection of cards,where each unique value appears 2 times among the
    /// cards  (forming a pair). The cards are then shuffled using the Fisher-Yates algorithm. If a seed is provided, 
    /// the shuffle will produce a Recreatable order, which can be useful for testing</remarks>
    /// <param name="pairCount">The number of unique card pairs to generate. Must be at least 1.</param>
    /// <param name="seed">An optional seed value for the random number generator used to shuffle the cards.  If none is provided, a random seed is used.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="pairCount"/> is less than 1.</exception>
    public void Initialize(int pairCount, int? seed = null)
    {
        if (pairCount < 1)
        {
            throw new ArgumentException("pairCount must be at least 1", nameof(pairCount));
        }

        _cards.Clear();

        //Generate values
        var values = Enumerable.Range(0, pairCount).Select(i => i.ToString()).ToList();

        //Create pairs 
        var allValues = values.Concat(values).ToList();

        //Shuffle
        //Using the Fisher-Yates shuffle algorithm (https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle)
        var random = seed.HasValue ? new Random(seed.Value): new Random();
        for (int i = allValues.Count - 1; i > 0; i--)
        {
            //Get 2nd random index
            int j = random.Next(0, i + 1);
            //Swapping the values of indexes i & J (Fisher-Yates), same as temp = i; i = j; j = temp;
            (allValues[i], allValues[j]) = (allValues[j], allValues[i]);
        }

        //Create cards with shuffled values and incrementting iDs
        for (int i = 0; i < allValues.Count; i++)
        {
            _cards.Add( new Card(i, allValues[i]) );
        }



    }


}