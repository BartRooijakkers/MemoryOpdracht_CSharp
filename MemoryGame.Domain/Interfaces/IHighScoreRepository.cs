using MemoryGame.Domain.Models;

namespace MemoryGame.Domain.Interfaces;
/// <summary>
/// Contract for saving and retrieving high scores. (max 10 items, sorted by score descending)
/// </summary>
public interface IHighScoreRepository
{
    /// <summary>
    /// Retrieve the top-10 high score entries, sorted by score in descending order. (same score entries are sorted by time first then,  date achieved ascending)
    /// </summary>
    IReadOnlyList<HighScoreEntry> GetTop10();


    /// <summary>
    /// Tries to add a entry to the top-10 high scores. If the entry's score qualifies for the top-10, it is added and the list is updated (and trimmed to only hold 10 scores) accordingly.
    /// </summary>
    /// <param name="entry">The suggested added Entry</param>
    /// <returns>(added: Yes/no, Rank: 1/10 if the entry made the top-10, -1 if it did not.</returns>
    (bool added, int rank) AddOrUpdateTop10(HighScoreEntry entry);

    /// <summary>
    /// Removes all high score entries from the repository. (Used for testing purposes)
    /// </summary>
    void Clear();
}