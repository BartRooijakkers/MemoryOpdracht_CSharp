namespace MemoryGame.Domain.Models;

public class Board
{
    public IReadOnlyList<Card> cards => _cards;
    private readonly List<Card> _cards;

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

        

    }

    public bool AllMatched()
    {
        //CalculateScore and enter in repository

        //komt zo
        return false;
    }

}