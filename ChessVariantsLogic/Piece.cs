namespace ChessVariantsLogic;

/// <summary>
/// Represents a piece with a fixed movement pattern.
/// </summary>
public class Piece
{
#region Fields, properties and constructors
    private readonly IMovementPattern movementPattern;
    private readonly bool royal;
    private readonly PieceClassifier pieceClassifier;
    private bool hasMoved;

    private string pieceIdentifier;

    private readonly int repeat;

    public bool Royal
    {
        get { return this.royal; }
    }

    public PieceClassifier PieceClassifier
    {
        get { return this.pieceClassifier;}
    }

    public IMovementPattern MovementPattern
    {
        get { return this.movementPattern; }
    }

    public int Repeat
    {
        get { return this.repeat; }
    }

    public string PieceIdentifier
    {
        get { return this.pieceIdentifier; }
    }

    /// <summary>
    /// Constructor for a new Piece.
    /// </summary>
    /// <param name="movementPattern">is the custom movement pattern</param>
    /// <param name="royal">set true if the piece is royal</param>
    /// <param name="pc">is the player the piece belongs to</param>
    /// <param name="hasMoved">set true if the piece has previously moved</param>
    /// <param name="repeat">is the amount of times the movement pattern can be repeated on the same turn</param>
    /// <param name="pieceIdentifier">is the unique string representation of the piece</param>
    public Piece(IMovementPattern movementPattern, bool royal, PieceClassifier pc, bool hasMoved, int repeat, string pieceIdentifier)
    {
        this.movementPattern = movementPattern;
        this.royal = royal;
        this.pieceClassifier = pc;
        this.hasMoved = hasMoved;
        this.repeat = repeat;
        this.pieceIdentifier = pieceIdentifier;
    }

    public Piece(IMovementPattern movementPattern, bool royal, PieceClassifier pc, int repeat, string pieceIdentifier)
    : this(movementPattern, royal, pc, false, repeat, pieceIdentifier) {}
    
    public Piece(IMovementPattern movementPattern, bool royal, PieceClassifier pc, string pieceIdentifier)
    : this(movementPattern, royal, pc, false, 0, pieceIdentifier) {}
#endregion

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

#region Static methods
    public static Piece Rook(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.North,
            Constants.East,
            Constants.South,
            Constants.West
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        if(pieceClassifier.Equals(PieceClassifier.WHITE))
            return new Piece(mp, false, pieceClassifier, Constants.WhiteRookIdentifier);
        return new Piece(mp, false, pieceClassifier, Constants.BlackRookIdentifier);
    }

    public static Piece Bishop(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.NorthEast,
            Constants.SouthEast,
            Constants.SouthWest,
            Constants.NorthWest
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth),
            new Tuple<int,int> (1, Constants.MaxBoardHeigth)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        if(pieceClassifier.Equals(PieceClassifier.WHITE))
            return new Piece(mp, false, pieceClassifier, Constants.WhiteBishopIdentifier);
        return new Piece(mp, false, pieceClassifier, Constants.BlackBishopIdentifier);
    }

    public static Piece Queen(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.North,
            Constants.East,
            Constants.South,
            Constants.West,
            Constants.NorthEast,
            Constants.SouthEast,
            Constants.SouthWest,
            Constants.NorthWest
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
        if(pieceClassifier.Equals(PieceClassifier.WHITE))
            return new Piece(mp, false, pieceClassifier, Constants.WhiteQueenIdentifier);
        return new Piece(mp, false, pieceClassifier, Constants.BlackQueenIdentifier);
    }

    public static Piece King(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.North,
            Constants.East,
            Constants.South,
            Constants.West,
            Constants.NorthEast,
            Constants.SouthEast,
            Constants.SouthWest,
            Constants.NorthWest
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
        if(pieceClassifier.Equals(PieceClassifier.WHITE))
            return new Piece(mp, true, pieceClassifier, Constants.WhiteKingIdentifier);
        return new Piece(mp, true, pieceClassifier, Constants.BlackKingIdentifier);
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
        if(pieceClassifier.Equals(PieceClassifier.WHITE))
            return new Piece(mp, false, pieceClassifier,Constants.WhiteKnightIdentifier);
        return new Piece(mp, false, pieceClassifier,Constants.BlackKnightIdentifier);
    }

    public static Piece BlackPawn()
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.South
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1,1)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        return new Piece(mp, false, PieceClassifier.BLACK, Constants.BlackPawnIdentifier);
    }

    public static Piece WhitePawn()
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.North
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1,1)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);

        return new Piece(mp, false, PieceClassifier.WHITE, Constants.WhitePawnIdentifier);
    }

    /// <summary>
    /// Instantiates a HashSet of all the standard pieces.
    /// </summary>
    /// <returns>A HashSet containing all the standard pieces.</returns>
    public static HashSet<Piece> AllStandardPieces()
    {
        return new HashSet<Piece> 
        {
            Rook(PieceClassifier.WHITE), Rook(PieceClassifier.BLACK),
            Knight(PieceClassifier.WHITE), Knight(PieceClassifier.BLACK),
            Bishop(PieceClassifier.WHITE), Bishop(PieceClassifier.BLACK),
            Queen(PieceClassifier.WHITE), Queen(PieceClassifier.BLACK),
            King(PieceClassifier.WHITE), King(PieceClassifier.BLACK),
            WhitePawn(), BlackPawn()
        };
    }
    
#endregion

}