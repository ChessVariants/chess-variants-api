namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern that allows jumping over other pieces.
/// </summary>
public class JumpMovementPattern : IMovementPattern
{
    private readonly List<Tuple<int, int>> movement;

#region Properties
    public List<Tuple<int,int>> Movement
    {
        get { return movement; }
    }

    public List<Tuple<int,int>> MoveLength
    {
        get { return new List<Tuple<int,int>>(); }
    }

#endregion

    public JumpMovementPattern(List<Tuple<int, int>> movement)
    {
        this.movement = movement;
    }
    public JumpMovementPattern() : this(new List<Tuple<int, int>>()) {}

    public void AddPattern(Tuple<int, int> direction)
    {
        this.movement.Add(direction);
    }

    public bool RemovePattern(Tuple<int, int> direction)
    {
        return this.movement.Remove(direction);
    }

}