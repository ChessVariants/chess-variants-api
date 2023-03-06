namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern for moving across a rectangular grid.
/// </summary>
public class MovementPattern
{
    private readonly List<IPattern> movement;

    /// <summary>
    /// Constructor creating a RegularMovementPattern instance. Elements in <paramref name="movement"/>"/>
    /// that corresponds to each other should have the same index.
    /// </summary>
    /// <param name="movement">is a list of all allowed movements.</param>
    public MovementPattern(List<IPattern> movement)
    {
        this.movement = movement;
    }

    public IEnumerable<IPattern> GetAllPatterns()
    {
        foreach (var pattern in movement)
        {
            yield return pattern;
        }
    }

#region Interface overrides

    public void AddPattern(IPattern pattern)
    {
        this.movement.Add(pattern);
    }

    public bool RemovePattern(IPattern pattern)
    {
        return this.movement.Remove(pattern);
    }

    public IPattern? GetPattern(int index)
    {
        if(index >= 0 && index < this.movement.Count)
            return this.movement[index];
        return null;
    }

    public int GetMovementPatternCount()
    {
        return this.movement.Count;
    }

#endregion

}