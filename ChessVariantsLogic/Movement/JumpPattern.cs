namespace ChessVariantsLogic;

/// <summary>
/// Represent a movement pattern with fixed range, which DOES allow jumping over occupied squares. Extends Pattern.
/// </summary>
public class JumpPattern : Pattern
{
    public JumpPattern(int xOffset, int yOffset) : base(xOffset, yOffset) {}

    public JumpPattern(Tuple<int,int> offsets) : this(offsets.Item1, offsets.Item2) {}

#region Overrides

    /// <summary>
    /// Exists to override MinLength from Pattern.
    /// </summary>
    /// <returns>-1</returns>
    public override int MinLength {get { return -1; } }

    /// <summary>
    /// Exists to override MaxLength from Pattern.
    /// </summary>
    /// <returns>-1</returns>
    public override int MaxLength { get { return -1; } }


    /// <inheritdoc/>
    public override bool Equals(Pattern? other)
    {
        if (other == null) return false;
        if(other is JumpPattern)
        {
            return this.XDir.Equals(other.XDir)
                && this.YDir.Equals(other.YDir);
        }
        return false;
    }

#endregion

}