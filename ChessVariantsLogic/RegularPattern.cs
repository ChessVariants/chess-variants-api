namespace ChessVariantsLogic;

public class RegularPattern : IPattern
{
    private readonly Tuple<int, int, int, int> pattern;

    public RegularPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this.pattern = new Tuple<int,int,int,int>(xDir, yDir, minLength, maxLength);
    }

#region Interface overrides
    public int GetXDir() { return pattern.Item1; }

    public int GetYDir() { return pattern.Item2; }

    public int GetXLength() { return pattern.Item3; }

    public int GetYLength() { return pattern.Item4; }
    
#endregion

}