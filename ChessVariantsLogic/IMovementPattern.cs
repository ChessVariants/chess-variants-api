namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern.
/// </summary>
public interface IMovementPattern
{
    /// <summary>
    /// Adds an additional pattern to the set of allowed moves.
    /// </summary>
    /// <param name="pattern"> is the pattern that is added to the set. </param>
    void AddPattern(Tuple<int, int> pattern, Tuple<int, int> moveLength);

    /// <summary>
    /// Removes a pattern from the set of allowed moves.
    /// </summary>
    /// <param name="pattern"> is the pattern that should be removed from the set.</param>
    /// <returns> true if remove was successful, otherwise false.</returns>
    bool RemovePattern(Tuple<int, int> pattern);

    /// <summary>
    /// Gets a specific pattern from the set of all allowed moves.
    /// </summary>
    /// <param name="index"> is the index where the pattern is located. </param>
    /// <returns> the pattern if <paramref name="index"/>   is in range, otherwise null. </returns>
    Tuple<int,int>? GetPattern(int index);

    /// <summary>
    /// Gets a specific move length connected to a specific pattern.
    /// </summary>
    /// <param name="index"> is the index where the move length is located.</param>
    /// <returns> the move length if <paramref name="index"/> is in range, otherwise null. </returns>
    Tuple<int,int>? GetMoveLength(Tuple<int, int> pattern);

    /// <summary>
    /// Gets the total number of allowed moves.
    /// </summary>
    /// <returns> the integer value representing the total number of allowed moves. </returns>
    int GetMovementPatternCount();

}