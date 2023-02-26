namespace ChessVariantsLogic;

public class MovementPattern
{
    private readonly List<Tuple<int, int>> movement;
    private bool jump;
    private (int, int) moveLength;
    private int repeat;

    public List<Tuple<int,int>> Movement
    {
        get { return movement; }
    }

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

    public MovementPattern(List<Tuple<int, int>> movement, bool jump, (int,int) moveLength, int repeat)
    {
        this.movement = movement;
        this.jump = jump;
        this.moveLength = moveLength;
        this.repeat = repeat;
    }



    public static Tuple<int, int> North = new Tuple<int, int>(-1,0);
    public static Tuple<int, int> East = new Tuple<int, int>(0,1);
    public static Tuple<int, int> South = new Tuple<int, int>(1,0);
    public static Tuple<int, int> West = new Tuple<int, int>(0,-1);
    public static Tuple<int, int> NorthEast = new Tuple<int, int>(-1,1);
    public static Tuple<int, int> SouthEast = new Tuple<int, int>(1,1);
    public static Tuple<int, int> SouthWest = new Tuple<int, int>(1,-1);
    public static Tuple<int, int> NorthWest = new Tuple<int, int>(-1,-1);

}