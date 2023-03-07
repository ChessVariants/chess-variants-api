namespace ChessVariantsLogic;

/// <summary>
/// Represent a movement pattern with fixed range, which DOES allow jumping over occupied squares. Implements IPattern.
/// </summary>
public class JumpPattern : IPattern
{
    private readonly Tuple<int, int> pattern;

    public JumpPattern(int xOffset, int yOffset)
    {
        this.pattern = new Tuple<int, int>(xOffset, yOffset);
    }

#region Interface overrides
    ///<inheritdoc /> 
    public int GetXDir() { return pattern.Item1; }

    /// <inheritdoc /> 
    public int GetYDir() { return pattern.Item2; }

    /// <summary>
    /// Exists to satisfy interface.
    /// </summary>
    /// <returns>-1</returns>
    public int GetMinLength() { return -1; }

    /// <summary>
    /// Exists to satisfy interface.
    /// </summary>
    /// <returns>-1</returns>
    public int GetMaxLength() { return -1; }

#endregion

}