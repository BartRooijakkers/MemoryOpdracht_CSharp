using MemoryGame.Domain.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MemoryGame.Wpf.ViewModels
{
    /// <summary>
    /// View model for a card, showing properties and providing ways to notify changes.
    /// </summary>
    /// <remarks>This class acts as a wrapper around the underlying <see cref="Card"/> model, exposing its
    /// properties in a way that supports data binding in WPF applications. It uses <see
    /// cref="INotifyPropertyChanged"/> to notify the UI of changes to the card's state.</remarks>
    public class CardViewModel : INotifyPropertyChanged
    {
        private readonly Card _card; // Underlying card model

        public int Id => _card.Id; // Expose card ID
        public string Value => _card.Value;// Expose card value
        public bool IsFaceUp => _card.IsFaceUp; // Expose if card is face up
        public bool IsMatched => _card.IsMatched; // Expose if card is matched

        public CardViewModel(Card card)
        {
            _card = card;
        }
        
        public void Refresh()
        {
            // Notify that properties may have changed
            OnPropertyChanged(nameof(IsFaceUp));
            OnPropertyChanged(nameof(IsMatched));
        }

        public event PropertyChangedEventHandler? PropertyChanged; // Event for property changes, nullable for safety.
        // Invoke the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string? name = null) 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}