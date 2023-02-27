namespace ChessVariantsLogic;

/// <summary>
/// Represents a piece with a fixed movement pattern.
/// </summary>
public class Piece
{
    private readonly IMovementPattern movementPattern;
    private readonly bool royal;
    private readonly PieceClassifier pieceClassifier;
    private bool hasMoved;

    private readonly int repeat;

#region Properties
    public bool Royal
    {
        get { return this.royal; }
    }

    public PieceClassifier PieceClassifier
    {
        get { return this.PieceClassifier;}
    }

    public IMovementPattern MovementPattern
    {
        get { return this.movementPattern; }
    }

    public int Repeat
    {
        get { return this.repeat; }
    }

#endregion

    public Piece(IMovementPattern movementPattern, bool royal, PieceClassifier pc, bool hasMoved, int repeat)
    {
        this.movementPattern = movementPattern;
        this.royal = royal;
        this.pieceClassifier = pc;
        this.hasMoved = hasMoved;
        this.repeat = repeat;
    }

    public Piece(IMovementPattern movementPattern, bool royal, PieceClassifier pc) : this(movementPattern, royal, pc, false, 0) {}

    /// <summary>
    /// Checks that two pieces are of opposite colors.
    /// </summary>
    /// <param name="p1">The first piece.</param>
    /// <param name="p2">The second piece.</param>
    /// <returns>true if pieces are of opposite colors.</returns>
    public static bool canTake(Piece p1, Piece p2)
    {
        return !p1.pieceClassifier.Equals(p2.pieceClassifier);
    }

#region Static fields
    public static Piece Rook(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            RegularMovementPattern.North,
            RegularMovementPattern.East,
            RegularMovementPattern.South,
            RegularMovementPattern.West
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece Bishop(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            RegularMovementPattern.NorthEast,
            RegularMovementPattern.SouthEast,
            RegularMovementPattern.SouthWest,
            RegularMovementPattern.NorthWest
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece Queen(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            RegularMovementPattern.North,
            RegularMovementPattern.East,
            RegularMovementPattern.South,
            RegularMovementPattern.West,
            RegularMovementPattern.NorthEast,
            RegularMovementPattern.SouthEast,
            RegularMovementPattern.SouthWest,
            RegularMovementPattern.NorthWest
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece King(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            RegularMovementPattern.North,
            RegularMovementPattern.East,
            RegularMovementPattern.South,
            RegularMovementPattern.West,
            RegularMovementPattern.NorthEast,
            RegularMovementPattern.SouthEast,
            RegularMovementPattern.SouthWest,
            RegularMovementPattern.NorthWest
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        return new Piece(mp, true, pieceClassifier);
    }

    public static Piece Knight(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            new Tuple<int, int>( 1, 2),
            new Tuple<int, int>( 2, 1),
            new Tuple<int, int>( 1,-2),
            new Tuple<int, int>( 2,-1),
            new Tuple<int, int>(-1, 2),
            new Tuple<int, int>(-2, 1),
            new Tuple<int, int>(-1,-2),
            new Tuple<int, int>(-2,-1),
        };
        var mp = new JumpMovementPattern(pattern);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece BlackPawn()
    {
        var pattern = new List<Tuple<int,int>> {
            RegularMovementPattern.South
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1,1)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        return new Piece(mp, false, PieceClassifier.BLACK);
    }

    public static Piece WhitePawn()
    {
        var pattern = new List<Tuple<int,int>> {
            RegularMovementPattern.North
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1,1)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        return new Piece(mp, false, PieceClassifier.WHITE);
    }
    
#endregion

}