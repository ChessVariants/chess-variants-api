namespace ChessVariantsLogic;

/// <summary>
/// Represent a movement pattern with fixed range, which DOES allow jumping over occupied squares. Implements IPattern.
/// </summary>
public class JumpPattern : IPattern
{
    private readonly int xDir;
    private readonly int yDir;

    public JumpPattern(int xOffset, int yOffset)
    {
        this.xDir = xOffset;
        this.yDir = yOffset;
    }

    public JumpPattern(Tuple<int,int> offsets) : this(offsets.Item1, offsets.Item2) {}

#region Interface overrides

    ///<inheritdoc /> 
    public int XDir {get {return this.xDir; } }
    
    /// <inheritdoc /> 
    public int YDir {get {return this.yDir; } }

    /// <summary>
    /// Exists to satisfy interface.
    /// </summary>
    /// <returns>-1</returns>
    public int MinLength {get {return -1; } }

    /// <summary>
    /// Exists to satisfy interface.
    /// </summary>
    /// <returns>-1</returns>
    public int MaxLength {get {return -1; } }

#endregion

}