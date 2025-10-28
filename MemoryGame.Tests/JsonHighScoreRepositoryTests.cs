using System;
using System.IO;
using System.Linq;
using MemoryGame.Data.Repositories;
using MemoryGame.Domain.Models;
using Xunit;

namespace MemoryGame.Tests
{
    public class JsonHighScoreRepositoryTests : IDisposable //Implement IDisposable to clean up temp files
    {
        private readonly string _path; //Temporary file path for testing
        private readonly JsonHighScoreRepository _repo; //Repository instance

        public JsonHighScoreRepositoryTests() //Constructor to set up temporary file and repository
        {
            _path = Path.Combine(Path.GetTempPath(), $"highscores_{Guid.NewGuid():N}.json"); //Unique temp file path
            _repo = new JsonHighScoreRepository(_path); //Initialize repository with temp file path
        }

        public void Dispose() //Dispose method to clean up temp file after tests
        {
            if (File.Exists(_path)) File.Delete(_path); //Delete temp file if it exists
        }
        // Helper method to create HighScoreEntry instances
        private static HighScoreEntry Make(string name, int score, int secs, int cards = 10, int attempts = 5)
            => new HighScoreEntry
            {
                PlayerName = name,
                Score = score,
                DurationSeconds = secs,
                Cards = cards,
                Attempts = attempts,
                DateAchieved = DateTime.Now
            };

        [Fact]
        public void GetTop10_Initially_ReturnsEmpty()
        {
            var list = _repo.GetTop10(); //Retrieve top-10 list
            Assert.Empty(list); //Assert that the list is initially empty
        }

        [Fact]
        public void AddOrUpdateTop10_AddsEntry_AndSortsDescendingOnScore()
        {
            var a = Make("A", 500, 30); // Lower score
            var b = Make("B", 900, 40); // Higher score

            var r1 = _repo.AddOrUpdateTop10(a); //Add first entry
            var r2 = _repo.AddOrUpdateTop10(b); // And second entry

            var list = _repo.GetTop10(); //Retrieve updated top-10 list
            Assert.True(r1.added); //Assert first entry was added
            Assert.True(r2.added);// Assert second entry was added
            Assert.Equal(2, list.Count); //Assert that there are now 2 entries
            Assert.Equal("B", list[0].PlayerName); //Assert that the higher score is first
            Assert.Equal(1, r2.rank); //Assert rank of second entry
        }

        [Fact]
        public void AddOrUpdateTop10_Trim_To_Max10()
        {
            // Add 11 entries to exceed the top-10 limit
            for (int i = 0; i < 11; i++)
                _repo.AddOrUpdateTop10(Make($"P{i}", 100 + i, 30)); //Add entries with increasing scores

            var list = _repo.GetTop10();
            Assert.Equal(10, list.Count); //Assert that only 10 entries remain
            // Assert that the lowest score (100) was removed
            Assert.DoesNotContain(list, e => e.Score == 100); //Score 100 should be removed
            Assert.Equal(110, list[0].Score); //Highest score should be 109
        }

        [Fact]
        public void AddOrUpdateTop10_TieBreaks_By_ShorterDuration_Then_OlderDateAchieved()
        {
            var old = Make("OldFast", 800, secs: 10); // Same score as others, but faster
            old.DateAchieved = DateTime.Now.AddMinutes(-5); // Achieved earlier

            var slow = Make("Slow", 800, secs: 20); // Same score, but slower
            var equalButOlder = Make("OlderSame", 800, secs: 10); // Same score and duration as 'old'
            equalButOlder.DateAchieved = DateTime.Now.AddMinutes(-10); // Achieved even earlier

            _repo.AddOrUpdateTop10(slow); // Add slow entry
            _repo.AddOrUpdateTop10(old); // Add fast entry
            _repo.AddOrUpdateTop10(equalButOlder); // Add equal but older entry

            var list = _repo.GetTop10();
            // Expected order: OlderSame, OldFast, Slow
            // Verify the order based on tie-breaking rules
            Assert.Equal("OlderSame", list[0].PlayerName); // Oldest among equals
            Assert.Equal("OldFast", list[1].PlayerName); // Fastest among equals
            Assert.Equal("Slow", list[2].PlayerName); // Slowest
        }

        [Fact]
        public void Clear_RemovesAll()
        {
            _repo.AddOrUpdateTop10(Make("X", 700, 15)); //Add an entry
            Assert.NotEmpty(_repo.GetTop10()); //Ensure it's added

            _repo.Clear(); //Clear all entries 
            Assert.Empty(_repo.GetTop10()); //Assert that the list is now empty
        }
    }
}
