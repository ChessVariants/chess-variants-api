namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern for moving across a rectangular grid.
/// </summary>
public class RegularMovementPattern : IMovementPattern
{
    private readonly List<Tuple<int, int>> movement;
    private readonly List<Tuple<int, int>> moveLength;

    public List<Tuple<int,int>> Movement
    {
        get { return movement; }
    }

#region Properties

    public List<Tuple<int, int>> MoveLength
    {
        get { return moveLength; }
    }

#endregion

    public RegularMovementPattern(List<Tuple<int, int>> movement, List<Tuple<int, int>> moveLength)
    {
        this.movement = movement;
        this.moveLength = moveLength;
    }

    public void AddPattern(Tuple<int, int> direction)
    {
        this.movement.Add(direction);
    }

    public bool RemovePattern(Tuple<int, int> direction)
    {
        return this.movement.Remove(direction);
    }

}