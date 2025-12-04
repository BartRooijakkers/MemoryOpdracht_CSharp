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


namespace MemoryGame.Wpf.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged // ViewModel for the game itself
    {
        private readonly Game _game = new(); // Underlying game model
        private readonly DispatcherTimer _uiTimer; // Timer to update elapsed time in UI

        public ObservableCollection<CardViewModel> Cards { get; } = new(); // Collection of card view models for data binding   

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
            _uiTimer.Tick += (_, __) => ElapsedText = Format(_game.ElapsedTime); // Update elapsed time text

            StartCommand = new RelayCommand(_ => StartGame()); // Command to start a new game
            FlipCardCommand = new RelayCommand(param => FlipCard(param)); // Command to flip a card
        }

        /// <summary>
        /// Starts a new game session with the specified number of card pairs. (Default = 5)
        /// </summary>
        /// <remarks>This method initializes the game state, clears any existing cards, and sets up the
        /// game timer.  The game begins immediately after this method is called.</remarks>
        /// <param name="pairCount">The number of card pairs to include in the game. The default value is 5.</param>
        private void StartGame(int pairCount = 5)
        {
            //TODO: Allow user to select pair count from textbox.
            _game.Start(pairCount); // Start the game in the model

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
                Trace.WriteLine($"Score: {_game.CalculateScore()}");
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
    }
}
