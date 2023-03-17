namespace ChessVariantsLogic;

/// <summary>
/// Represent a movement pattern with dynamic range, which DOES NOT allow jumping over occupied squares. Implements IPattern.
/// </summary>
public class RegularPattern : IPattern
{
    private readonly int xDir;
    private readonly int yDir;
    private readonly int minLength;
    private readonly int maxLength;

    private readonly Tuple<int, int, int, int> pattern;

    public RegularPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this.xDir = xDir;
        this.yDir = yDir;
        this.minLength = minLength;
        this.maxLength = maxLength;
        this.pattern = new Tuple<int,int,int,int>(xDir, yDir, minLength, maxLength);
    }

    public RegularPattern(Tuple<int,int> direction, int minLength, int maxLength) : this(direction.Item1, direction.Item2, minLength, maxLength) {}

#region Interface overrides

    /// <inheritdoc /> 
    public int XDir {get {return this.xDir; } }
    /// <inheritdoc /> 
    public int YDir {get {return this.yDir; } }
    /// <inheritdoc /> 
    public int MinLength {get {return this.minLength; } }
    /// <inheritdoc /> 
    public int MaxLength {get {return this.maxLength; } }

#endregion

}