namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern for moving across a rectangular grid.
/// </summary>
public class RegularMovementPattern : IMovementPattern
{
    private readonly List<Tuple<int, int>> movement;
    private readonly List<Tuple<int, int>> moveLength;

    public RegularMovementPattern(List<Tuple<int, int>> movement, List<Tuple<int, int>> moveLength)
    {
        this.movement = movement;
        this.moveLength = moveLength;
    }

#region Interface overrides

    public void AddPattern(Tuple<int, int> pattern)
    {
        this.movement.Add(pattern);
    }

    public bool RemovePattern(Tuple<int, int> pattern)
    {
        return this.movement.Remove(pattern);
    }

    public Tuple<int,int>? GetMovement(int index)
    {
        if(index >= 0 && index < this.movement.Count)
            return this.movement[index];
        return null;
    }

    public Tuple<int,int>? GetMoveLength(int index)
    {
        if(index >= 0 && index < this.movement.Count)
            return this.moveLength[index];
        return null;
    }

    public int GetMovementPatternCount()
    {
        return this.movement.Count;
    }
#endregion

}