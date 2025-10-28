using System.Linq;

namespace MemoryGame.Domain.Models;


public class Board
{
    public IReadOnlyList<Card> Cards => _cards; //Readonly exposure of cards
    private readonly List<Card> _cards = new(); //Internal list of cards

    //If Board is not empty and all cards are matched
    public bool AllMatched() => _cards.Count > 0 && _cards.All(c => c.IsMatched);

    public Board()
    {

    }

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