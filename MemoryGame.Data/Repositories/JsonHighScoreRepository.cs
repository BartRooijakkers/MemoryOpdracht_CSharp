using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MemoryGame.Domain.Interfaces;
using MemoryGame.Domain.Models;

namespace MemoryGame.Data.Repositories;

/// <summary>
/// JSON-Implementation of the high score repository. Saves scores to a local file in JSON format. Ensures that the top-10 high scores are maintained.
/// </summary>
public class JsonHighScoreRepository : IHighScoreRepository
{
    private readonly string _filePath; //Path to the JSON file
    private readonly JsonSerializerOptions _jsonOptions = new()  
    {
        WriteIndented = true // For better readability
    };
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonHighScoreRepository"/> class with an optional file path for
    /// storing high scores.
    /// </summary>
    /// <param name="filePath">The path to the JSON file where high scores will be stored. If <see langword="null"/>, a default path is used if none is provided.
    /// </param>
    public JsonHighScoreRepository(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "highscoresMemory.json"); // Use filepath, but if its null. Use default path.
    }

    public IReadOnlyList<HighScoreEntry> GetTop10()
    {
        if(!File.Exists(_filePath)) //Check if file exists
        {
            return new List<HighScoreEntry>(); //Return empty list if not
        }
        var json = File.ReadAllText(_filePath); //Read file content
        var list = JsonSerializer.Deserialize<List<HighScoreEntry>>(json, _jsonOptions) ?? new List<HighScoreEntry>(); //Deserialize JSON to list, If it fails return empty list.
        return SortAndTrim(list); //Sort and trim to top-10 before returning
    }

    public (bool added, int rank) AddOrUpdateTop10(HighScoreEntry entry)
    {
        var list = GetTop10().ToList(); //Get current top-10 list
        list.Add(entry); //Add new entry
        list = SortAndTrim(list);//Sort: Highest score first, then by duration (lowest first), then by date achieved (oldest first) and trim to top-10
        bool added = list.Any(e => e == entry);
        int rank = added ? list.IndexOf(entry) +1 : -1; //Calculate rank if added, else -1 (rank is 1-10 and positions are 0-9. So +1)
        Save(list); //Save updated list
        return (added, rank);//Return whether added and rank
    }

    public void Clear()
    {
        if(File.Exists(_filePath)) //Check if file exists
        {
            File.Delete(_filePath); //Delete the file to clear scores
        }
    }

    private void Save(List<HighScoreEntry> list)
    {
        var json = JsonSerializer.Serialize(list, _jsonOptions); //Serialize list to JSON using options.
        File.WriteAllText(_filePath, json); //Write JSON to file
    }
    /// <summary>
    /// Sorts a list of high score entries in descending order by score, then by ascending duration,  and then by
    /// ascending date achieved, and returns the top 10 entries.
    /// </summary>
    /// <param name="list">The list of high score entries to sort and trim. Cannot be null.</param>
    /// <returns>A list containing the top 10 high score entries..  If the input list contains
    /// fewer than 10 entries, all entries are returned in sorted order.</returns>
    private static List<HighScoreEntry> SortAndTrim(List<HighScoreEntry> list)
    {
        var sorted = list.OrderByDescending(e => e.Score) //Sort by score descending
            .ThenBy(e => e.DurationSeconds). //Then by duration ascending
            ThenBy(e => e.DateAchieved). //Then by date achieved ascending
            Take(10). //Take top-10
            ToList(); //Convert to list

        return sorted; //Return sorted and trimmed list

    }

}