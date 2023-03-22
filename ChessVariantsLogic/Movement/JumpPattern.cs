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


    /// <summary>
    /// Returns a bool indicating wheter this and <paramref name="other"/> are equal.
    /// </summary>
    /// <param name="other"> is the other IPattern to be compared to this.</param>
    /// <returns>true if this is equals to <paramref name="other"/>, otherwise false.</returns>
    public bool Equals(IPattern? other)
    {
        if (other == null) return false;
        return this.XDir.Equals(other.XDir)
            && this.YDir.Equals(other.YDir);
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
        var objAsPattern = other as JumpPattern;
        if(objAsPattern == null)
            return false;
        return this.Equals(objAsPattern);
    }
    /// <summary>
    /// Returns the hash code for the current JumpPattern object.
    /// </summary>
    /// <returns>An int representing the hash code of the object.</returns>
    public override int GetHashCode()
    {
        return Tuple.Create(this.xDir, this.yDir).GetHashCode();
    }

#endregion
}