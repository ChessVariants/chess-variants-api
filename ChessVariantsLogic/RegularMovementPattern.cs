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


#region Static fields
    public static Tuple<int, int> North = new Tuple<int, int>(-1,0);
    public static Tuple<int, int> East = new Tuple<int, int>(0,1);
    public static Tuple<int, int> South = new Tuple<int, int>(1,0);
    public static Tuple<int, int> West = new Tuple<int, int>(0,-1);
    public static Tuple<int, int> NorthEast = new Tuple<int, int>(-1,1);
    public static Tuple<int, int> SouthEast = new Tuple<int, int>(1,1);
    public static Tuple<int, int> SouthWest = new Tuple<int, int>(1,-1);
    public static Tuple<int, int> NorthWest = new Tuple<int, int>(-1,-1);

#endregion

}