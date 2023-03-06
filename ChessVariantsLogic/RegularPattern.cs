namespace ChessVariantsLogic;

/// <summary>
/// Represent a movement pattern with dynamic range, which DOES NOT allow jumping over occupied squares. Implements IPattern.
/// </summary>
public class RegularPattern : IPattern
{
    private readonly Tuple<int, int, int, int> pattern;

    public RegularPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this.pattern = new Tuple<int,int,int,int>(xDir, yDir, minLength, maxLength);
    }

    public RegularPattern(Tuple<int,int> direction, int minLength, int maxLength) : this(direction.Item1, direction.Item2, minLength, maxLength) {}

#region Interface overrides
    public int GetXDir() { return pattern.Item1; }

    public int GetYDir() { return pattern.Item2; }

    public int GetMinLength() { return pattern.Item3; }

    public int GetMaxLength() { return pattern.Item4; }

#endregion

}