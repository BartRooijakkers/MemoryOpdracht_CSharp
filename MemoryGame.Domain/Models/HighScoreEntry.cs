namespace MemoryGame.Domain.Models;

public class HighScoreEntry
{
    public string PlayerName { get; set; } = ""; //Player name associated with the high score, default is empty
    public int Score { get; set; } // The score achieved by the player, as calculated by the ScoreCalculator
    public int Cards { get; set; } // The total number of cards in the game when the score was achieved
    public int Attempts { get; set; } // The number of attempts taken to complete the game
    public int DurationSeconds { get; set; }// The duration of the game in seconds
    public DateTime DateAchieved { get; set; } = DateTime.Now; // The date and time when the high score was achieved, default is current time

    public HighScoreEntry(string playerName, int score, int cards, int attempts, int durationSeconds, DateTime dateAchieved)
    {
        PlayerName = playerName;
        Score = score;
        Cards = cards;
        Attempts = attempts;
        DurationSeconds = durationSeconds;
        DateAchieved = dateAchieved;
    }
}