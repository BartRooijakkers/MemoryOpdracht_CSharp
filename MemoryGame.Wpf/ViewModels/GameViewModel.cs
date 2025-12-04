using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using MemoryGame.Domain.Models;
using MemoryGame.Wpf;               // RelayCommand
using MemoryGame.Wpf.ViewModels;    // CardViewModel

using System.Diagnostics;
using MemoryGame.Data.Repositories;
using MemoryGame.Domain.Interfaces;


namespace MemoryGame.Wpf.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged // ViewModel for the game itself
    {
        private readonly Game _game = new(); // Underlying game model
        private readonly DispatcherTimer _uiTimer; // Timer to update elapsed time in UI
        private readonly IHighScoreRepository _highScoreRepo = new JsonHighScoreRepository(); // repository for high scores

        public ObservableCollection<CardViewModel> Cards { get; } = new(); // Collection of card view models for data binding   

        //Paircount property to allow user selection of pair count
        private int _pairCount = 5; // (default) Number of card pairs
        public int PairCount
        {
            get => _pairCount;
            set
            {
                _pairCount = value; OnPropertyChanged(); // Notify UI of changes
            } 
        }
        //Player name property to allow user input for player name
        private string _playerName = "Bart";
        public string PlayerName
        {
            get => _playerName;
            set { _playerName = value; OnPropertyChanged(); }
        }

        // High scores collection for data binding - loaded from repository.
        // This will allow me to bind the high scores to a UI element.
        public ObservableCollection<HighScoreEntry> HighScores { get; } = new();


        private int _attempts;// Number of attempts made
        public int Attempts
        {
            get => _attempts;
            private set { _attempts = value; OnPropertyChanged(); } // Notify UI of changes
        }

        private string _elapsedText = "00:00"; // Elapsed time as text
        public string ElapsedText
        {
            get => _elapsedText;
            private set { _elapsedText = value; OnPropertyChanged(); } // Notify UI of changes
        }

        private bool _isCompleted; // Whether the game is completed
        public bool IsCompleted
        {
            get => _isCompleted;
            private set { _isCompleted = value; OnPropertyChanged(); } // Notify UI of changes
        }

        // Commands
        public RelayCommand StartCommand { get; }
        public RelayCommand FlipCardCommand { get; }

        public GameViewModel()
        {
            // UI-timer to update elapsed time every 250 ms
            _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) }; // Set interval
            _uiTimer.Tick += (_, __) => ElapsedText = Format(_game.ElapsedTime); // Update elapsed time text -- Hoogie

            StartCommand = new RelayCommand(_ => StartGame()); // Command to start a new game
            FlipCardCommand = new RelayCommand(param => FlipCard(param)); // Command to flip a card

            LoadHighScores(); // Load high scores from repository
        }

        /// <summary>
        /// Starts a new game session with the specified number of card pairs. (Default = 5)
        /// </summary>
        /// <remarks>This method initializes the game state, clears any existing cards, and sets up the
        /// game timer.  The game begins immediately after this method is called.</remarks>
        /// <param name="pairCount">The number of card pairs to include in the game. The default value is 5.</param>
        private void StartGame()
        {
            _game.Start(PairCount); // Start the game in the model

            Cards.Clear(); // Clear existing cards
            foreach (var c in _game.Cards) // Iterate over model cards
                Cards.Add(new CardViewModel(c)); // Add new card view models, wrapping model cards so UI can bind to them and get notified of changes

            Attempts = 0; // Reset attempts
            IsCompleted = false; // Reset completion status
            ElapsedText = "00:00";// Reset elapsed time text
            _uiTimer.Start();// Start the UI timer
        }
        /// <summary>
        /// Flips the card with the specified identifier and updates the game state.
        /// </summary>
        /// <remarks>This method updates the visual state of the cards and refreshes the card view models,
        /// and updates the game statistics like the number of attempts and the completion status. If the game is
        /// completed, the UI timer is stopped.</remarks>
        /// <param name="parameter">The identifier of the card to flip. This can be an <see cref="int"/> representing the card ID  or a <see
        /// cref="string"/> that can be parsed into an integer. If the parameter is null or invalid, the method does
        /// nothing.</param>
        /// Added Async to Hoogie
        private async void FlipCard(object? parameter)
        {
            // Validate and parse parameter
            if (parameter is null) return;

            // Determine card ID from parameter
            int id = parameter switch
            {
                int i => i, // Direct int
                string s when int.TryParse(s, out var parsed) => parsed, // Try parse string to int
                _ => -1 // Invalid type
            };
            if (id < 0){
                return; // Invalid ID, do nothing
            }

            //Prevent ClickSpamming when there is a pending mismatch Hoogie
            if (_game.HasPendingMismatch)
            {
                return; // Do nothing if there is a pending mismatch to resolve Hoogie
            }

            Trace.WriteLine($"Flipping card with ID: {id}"); // Debug output
            _game.FlipCard(id); // Flip the card in the model
            Trace.WriteLine($"Card with ID: {id} flipped."); // Debug output

            // Refresh the specific card view model
            var vm = Cards.FirstOrDefault(x => x.Id == id);
            vm?.Refresh();

            // Refresh all cards to ensure UI is up to date
            foreach (var c in Cards)
            {
                c.Refresh();
            }

            // Update game stats
            Attempts = _game.Attempts;
            IsCompleted = _game.IsCompleted;

            //Mismatch delay handling Hoogie
            if (_game.HasPendingMismatch)
            {
                await Task.Delay(500); //Wait for 0.5 seconds before flipping back
                _game.ResolvePendingMismatch(); // Resolve the mismatch in the model
                // Refresh all cards again after resolving mismatch
                foreach (var c in Cards)
                {
                    c.Refresh();
                }
            }

            // Stop UI timer if game is completed
            if (IsCompleted)
            {
                _uiTimer.Stop();
                Trace.WriteLine("Game completed! Stopping timer."); // Debug output

                var (added, rank, entry) = _game.SaveHighScore(_highScoreRepo, PlayerName);

                LoadHighScores(); // Reload high scores from repository, If new score has been added it will show up

                if (added)
                {
                    Trace.WriteLine($"Highscore! Rank: {rank}, Score: {entry.Score}");
                }
                else
                {
                    Trace.WriteLine("No highscore achieved.");
                }

            }
        }

        private static string Format(TimeSpan t)
        {
            int mm = (int)t.TotalMinutes; // Total minutes
            int ss = t.Seconds; // Seconds component
            return $"{mm:00}:{ss:00}"; // Format as MM:SS
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        // Load high scores from repository
        private void LoadHighScores()
        {
            HighScores.Clear();
            foreach (var hs in _highScoreRepo.GetTop10())
                HighScores.Add(hs);
        }

    }
}
