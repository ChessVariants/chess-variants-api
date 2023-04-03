namespace ChessVariantsLogic;

/// <summary>
/// An interface for different types of movement patterns.
/// </summary>
public abstract class Pattern : IEquatable<Pattern>
{

    private readonly int xDir;
    private readonly int yDir;
    private readonly int minLength;
    private readonly int maxLength;

    public Pattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this.xDir = xDir != 0 ? xDir/Math.Abs(xDir) : 0;
        this.yDir = yDir != 0 ? yDir/Math.Abs(yDir) : 0;
        this.minLength = minLength;
        this.maxLength = maxLength;
    }
    public Pattern(int xDir, int yDir)
    {
        this.xDir = xDir;
        this.yDir = yDir;
        this.minLength = -1;
        this.maxLength = -1;
    }

    /// <summary>
    /// Gets the direction in the X-axis.
    /// </summary>
    /// <returns>the direction in the X-axis as an integer.</returns>
    public int XDir { get { return this.xDir; } }

    /// <summary>
    /// Gets the direction in the Y-axis.
    /// </summary>
    /// <returns>the direction in the Y-axis as an integer.</returns>
    public int YDir { get { return this.yDir; } }

    /// <summary>
    /// Gets the minimum length that this pattern requires to be travelled.
    /// </summary>
    /// <returns>the minimuim length as an integer.</returns>
    public virtual int MinLength { get { return this.minLength; } }

    /// <summary>
    /// Gets the maximum length that this pattern allows to be travelled.
    /// </summary>
    /// <returns>the maximum length as an integer.</returns>
    public virtual int MaxLength { get { return this.maxLength; } }

    /// <summary>
    /// Returns a bool indicating wheter this and <paramref name="other"/> are equal.
    /// </summary>
    /// <param name="other"> is the other IPattern to be compared to this.</param>
    /// <returns>true if this is equals to <paramref name="other"/>, otherwise false.</returns>
    public abstract bool Equals(Pattern? other);

    /// <summary>
    /// Returns a bool indicating wheter this and <paramref name="other"/> are equal.
    /// </summary>
    /// <param name="other"> is the other object to be compared to this.</param>
    /// <returns>true if this is equal to <paramref name="other"/>, otherwise false.</returns>
    public override bool Equals(Object? other)
    {
        if (other == null)
            return false;
        var objAsPattern = other as Pattern;
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

}