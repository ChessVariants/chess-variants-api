namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern that allows jumping over other pieces.
/// </summary>
public class JumpMovementPattern : IMovementPattern
{
    private readonly List<Tuple<int, int>> movement;

    public JumpMovementPattern(List<Tuple<int, int>> movement)
    {
        this.movement = movement;
    }
    public JumpMovementPattern() : this(new List<Tuple<int, int>>()) {}

#region Interface overrides
    public void AddPattern(Tuple<int, int> pattern, Tuple<int, int> moveLength)
    {
        this.movement.Add(pattern);
    }

    public bool RemovePattern(Tuple<int, int> pattern)
    {
        return this.movement.Remove(pattern);
    }

    public Tuple<int,int>? GetPattern(int index)
    {
        if(index >= 0 && index < this.movement.Count)
            return this.movement[index];
        return null;
    }

    public Tuple<int,int>? GetMoveLength(Tuple<int, int> pattern)
    {
        return null;
    }

    public int GetMovementPatternCount()
    {
        return this.movement.Count;
    }

#endregion

}