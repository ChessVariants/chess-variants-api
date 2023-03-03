namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern for moving across a rectangular grid.
/// </summary>
public class RegularMovementPattern : IMovementPattern
{
    private readonly List<Tuple<int, int>> movement;
    private readonly List<Tuple<int, int>> moveLength;

    private readonly Dictionary<Tuple<int,int>, Tuple<int,int>> patternToMoveLength;

    /// <summary>
    /// Constructor creating a RegularMovementPattern instance. Elements in <paramref name="movement"/> and <paramref name="moveLength"/>
    /// that corresponds to each other should have the same index.
    /// </summary>
    /// <param name="movement">is a list of all allowed movements.</param>
    /// <param name="moveLength">is a list of the lengths of the corresponding move.</param>
    public RegularMovementPattern(List<Tuple<int, int>> movement, List<Tuple<int, int>> moveLength)
    {
        this.movement = movement;
        this.moveLength = moveLength;
        this.patternToMoveLength = initPatternToMoveLength();
    }

#region Interface overrides

    public void AddPattern(Tuple<int, int> pattern, Tuple<int, int> moveLength)
    {
        this.movement.Add(pattern);
        this.patternToMoveLength.Add(pattern, moveLength);
    }

    public bool RemovePattern(Tuple<int, int> pattern)
    {
        bool result = false;
        try
        {
            result = this.patternToMoveLength.Remove(pattern);
        }
        catch (ArgumentNullException) {}

        return result;
    }

    public Tuple<int,int>? GetPattern(int index)
    {
        if(index >= 0 && index < this.movement.Count)
            return this.movement[index];
        return null;
    }

    public Tuple<int,int>? GetMoveLength(Tuple<int, int> pattern)
    {
        Tuple<int,int>? moveLength = null;
        try
        {
            moveLength = this.patternToMoveLength[pattern];
        }
        catch (KeyNotFoundException) {}

        return moveLength;
    }

    public int GetMovementPatternCount()
    {
        return this.movement.Count;
    }
#endregion

    // Initializes the dictionary patternToMoveLength.
    private Dictionary<Tuple<int,int>, Tuple<int,int>> initPatternToMoveLength()
    {
        var ptml = new Dictionary<Tuple<int,int>, Tuple<int,int>>();
        int minIndex = Math.Min(this.moveLength.Count, this.movement.Count);

        for(int i = 0; i < minIndex; i++)
        {
            try
            {
                ptml.Add(this.movement[i], moveLength[i]);
            }
            catch (ArgumentException) {}
        }

        return ptml;
    }

}