namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern for traversing a rectangular grid.
/// </summary>
public class MovementPattern
{
    private readonly HashSet<Pattern> movement;
    public int Count { get { return this.movement.Count; } }

    /// <summary>
    /// Constructor creating a RegularMovementPattern instance. Elements in <paramref name="movement"/>"/>
    /// that corresponds to each other should have the same index.
    /// </summary>
    /// <param name="movement">is a list of all allowed movements.</param>
    public MovementPattern(HashSet<Pattern> movement)
    {
        this.movement = movement;
    }

    public MovementPattern()
    {
        this.movement = new HashSet<Pattern>();
    }

    /// <summary>
    /// Gets all IPatterns in this MovementPattern as an IEnumberable.
    /// </summary>
    /// <returns> all IPatterns individually.</returns>
    public IEnumerable<Pattern> GetAllPatterns()
    {
        foreach (var pattern in movement)
        {
            yield return pattern;
        }
    }

    /// <summary>
    /// Adds an IPattern to this movement IPattern.
    /// </summary>
    /// <param name="pattern"> is the IPattern that is added.</param>
    public void AddPattern(Pattern pattern)
    {
        this.movement.Add(pattern);
    }

    /// <summary>
    /// Removes the IPattern from this MovementPattern.
    /// </summary>
    /// <param name="pattern"> is the IPattern that should be removed.</param>
    /// <returns>true if the IPattern was contained in this MovementPattern, otherwise false.</returns>
    public bool RemovePattern(Pattern pattern)
    {
        return this.movement.Remove(pattern);
    }

}