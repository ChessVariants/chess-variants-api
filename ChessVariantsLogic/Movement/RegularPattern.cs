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

    public RegularPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        // Can not move more than one step at this time, perhaps experiment with greater values later.
        // Could possibly allow for a piece to move only on every other row, i.e. like a dark-squared rook.
        this.xDir = xDir != 0 ? xDir/Math.Abs(xDir) : 0;
        this.yDir = yDir != 0 ? yDir/Math.Abs(yDir) : 0;
        this.minLength = minLength;
        this.maxLength = maxLength;
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

    /// <summary>
    /// Returns a bool indicating wheter this and <paramref name="other"/> are equal.
    /// </summary>
    /// <param name="other"> is the other IPattern to be compared to this.</param>
    /// <returns>true if this is equals to <paramref name="other"/>, otherwise false.</returns>
    public bool Equals(IPattern? other)
    {
        if (other == null) return false;
        return this.XDir.Equals(other.XDir)
            && this.YDir.Equals(other.YDir)
            && this.minLength.Equals(other.MinLength)
            && this.maxLength.Equals(other.MaxLength);
    }

#endregion

#region Object overrides
    /// <summary>
    /// Returns a bool indicating wheter this and <paramref name="other"/> are equal.
    /// </summary>
    /// <param name="other"> is the other object to be compared to this.</param>
    /// <returns>true if this is equal to <paramref name="other"/>, otherwise false.</returns>
    public override bool Equals(Object? other)
    {
        if (other == null)
            return false;
        var objAsPattern = other as RegularPattern;
        if(objAsPattern == null)
            return false;
        return this.Equals(objAsPattern);
    }

    /// <summary>
    /// Returns the hash code for the current RegularPattern object.
    /// </summary>
    /// <returns>An int representing the hash code of the object.</returns>
    public override int GetHashCode()
    {
        return Tuple.Create(this.xDir, this.yDir, this.minLength, this.maxLength).GetHashCode();
    }

#endregion

}