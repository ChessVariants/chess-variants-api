namespace ChessVariantsLogic;

/// <summary>
/// Represents a movement pattern for moving across a rectangular grid.
/// </summary>
public class MovementPattern
{
    private readonly List<Tuple<int, int>> movement;
    private readonly bool jump;
    private readonly (int, int) moveLength;
    private readonly int repeat;

    public List<Tuple<int,int>> Movement
    {
        get { return movement; }
    }

#region Properties
    public bool Jump
    {
        get { return jump; }
    }

    public (int,int) MoveLength
    {
        get { return moveLength; }
    }

    public int Repeat
    {
        get { return repeat; }
    }

#endregion

    public MovementPattern(List<Tuple<int, int>> movement, bool jump, (int,int) moveLength, int repeat)
    {
        this.movement = movement;
        this.jump = jump;
        this.moveLength = moveLength;
        this.repeat = repeat;
    }

    public MovementPattern(bool jump, (int,int) moveLength, int repeat) : this(new List<Tuple<int, int>>(), jump, moveLength, repeat) {}
    public MovementPattern(bool jump, (int,int) moveLength) : this(new List<Tuple<int, int>>(), jump, moveLength, 0) {}

    public void Add(Tuple<int, int> direction)
    {
        this.movement.Add(direction);
    }

    public bool Remove(Tuple<int, int> direction)
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