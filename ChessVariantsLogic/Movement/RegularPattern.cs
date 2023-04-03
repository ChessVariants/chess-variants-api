namespace ChessVariantsLogic;

/// <summary>
/// Represent a movement pattern with dynamic range, which DOES NOT allow jumping over occupied squares. Extends Pattern.
/// </summary>
public class RegularPattern : Pattern
{

    public RegularPattern(int xDir, int yDir, int minLength, int maxLength) : base(xDir, yDir, minLength, maxLength) {}

    public RegularPattern(Tuple<int,int> direction, int minLength, int maxLength) : this(direction.Item1, direction.Item2, minLength, maxLength) {}

    /// <inheritdoc/>
    public override bool Equals(Pattern? other)
    {
        if (other == null) return false;
        if(other is RegularPattern)
        {
            return this.XDir.Equals(other.XDir)
                && this.YDir.Equals(other.YDir)
                && this.MinLength.Equals(other.MinLength)
                && this.MaxLength.Equals(other.MaxLength);
        }
        return false;
    }
}