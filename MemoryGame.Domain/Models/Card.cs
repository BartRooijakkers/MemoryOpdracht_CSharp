namespace MemoryGame.Domain.Models;

//Could be using an enum for the 'cardstate' (Face being up or down, IsMatched or not) here, but keeping it simple for now
/// <summary>
/// A class representing a single card in the memory game.
/// </summary>
public class Card
{

    public int Id { get;}
    public string Value { get;}
    /// <summary>
    /// Gets a value indicating whether the card is in the "face-up" state.
    /// </summary>
    public bool IsFaceUp { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the current card is matched with its pair.
    /// </summary>
    public bool IsMatched { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Card"/> class with the specified identifier and value.
    /// </summary>
    /// <param name="id">The unique identifier for the card.</param>
    /// <param name="value">The value or symbol associated with the card.</param>
    public Card(int id, string value)
    {
        Id = id;
        Value = value;
        IsFaceUp = false;
        IsMatched = false;
    }
    /// <summary>
    /// Flips the card to toggle its face-up state, unless the card is already matched, in which case it remains face-up.
    /// </summary>
    /// <remarks>If the card is matched, its face-up state cannot be changed. Otherwise, this method toggles
    /// the <see cref="IsFaceUp"/> property between <see langword="true"/> and  <see langword="false"/>.</remarks>
    public void Flip()
    {
        if (!IsMatched)
        {
            IsFaceUp = !IsFaceUp;
        }
    }
    /// <summary>
    /// Marks the current object as matched and sets its state to face up.
    /// </summary>
    /// <remarks>This method updates the <see cref="IsMatched"/> property to <see langword="true"/>  and
    /// ensures the <see cref="IsFaceUp"/> property is also set to <see langword="true"/>.  If the object is already
    /// matched, no changes are made.</remarks>
    public void Match()
    {
        if (!IsMatched)
        {
            IsMatched = true;
            IsFaceUp = true;
        }
    }

}