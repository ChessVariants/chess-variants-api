namespace ChessVariantsLogic;

/// <summary>
/// Represents a piece with a fixed movement pattern.
/// </summary>
public class Piece
{
#region Fields, properties and constructors
    private readonly IMovementPattern movementPattern;
    private readonly IMovementPattern capturePattern;
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

    public IMovementPattern CapturePattern
    {
        get { return this.capturePattern; }
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
    public Piece(IMovementPattern movementPattern, IMovementPattern capturePattern, bool royal, PieceClassifier pc, bool hasMoved, int repeat, string pieceIdentifier)
    {
        this.movementPattern = movementPattern;
        this.capturePattern = capturePattern;
        this.royal = royal;
        this.pieceClassifier = pc;
        this.hasMoved = hasMoved;
        this.repeat = repeat;
        this.pieceIdentifier = pieceIdentifier;
    }

    public Piece(IMovementPattern movementPattern, IMovementPattern capturePattern, bool royal, PieceClassifier pc, int repeat, string pieceIdentifier)
    : this(movementPattern, capturePattern, royal, pc, false, repeat, pieceIdentifier) {}
    
    public Piece(IMovementPattern movementPattern, IMovementPattern capturePattern, bool royal, PieceClassifier pc, string pieceIdentifier)
    : this(movementPattern, capturePattern, royal, pc, false, 0, pieceIdentifier) {}

#endregion

    /// <summary>
    /// Gets a specific movement pattern by index.
    /// </summary>
    /// <param name="index" is the index of the movement pattern></param>
    /// <returns> the movement pattern at <paramref name="index"/>.</returns>
    public Tuple<int,int>? GetMovementPattern(int index)
    {
        return this.movementPattern.GetMovement(index);
    }

    public Tuple<int,int>? GetCapturePattern(int index)
    {
        return this.capturePattern.GetMovement(index);
    }

    /// <summary>
    /// Gets a specific move length by index.
    /// </summary>
    /// <param name="index"> is the index of the move length</param>
    /// <returns> the move length at index <paramref name="index"/>.</returns>
    public Tuple<int, int>? GetMoveLength(int index)
    {
        return this.movementPattern.GetMoveLength(index);
    }

    public Tuple<int, int>? GetCaptureLength(int index)
    {
        return this.capturePattern.GetMoveLength(index);
    }

    /// <summary>
    /// Gets the total number of moves that this piece can perform on an empty board.
    /// </summary>
    /// <returns> the total number of moves this piece can perform. </returns>
    public int GetMovementPatternCount()
    {
        return this.movementPattern.GetMovementPatternCount();
    }

    public int GetCapturePatternCount()
    {
        return this.capturePattern.GetMovementPatternCount();
    }

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

    /// <summary>
    /// Creates a Piece object that behaves like a standard rook.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the rook belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard rook.</returns>
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
            return new Piece(mp, mp,  false, pieceClassifier, Constants.WhiteRookIdentifier);
        return new Piece(mp, mp, false, pieceClassifier, Constants.BlackRookIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard bishop.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the bishop belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard bishop.</returns>
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
            return new Piece(mp, mp, false, pieceClassifier, Constants.WhiteBishopIdentifier);
        return new Piece(mp, mp, false, pieceClassifier, Constants.BlackBishopIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard queen.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the queen belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard queen.</returns>
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
            return new Piece(mp, mp, false, pieceClassifier, Constants.WhiteQueenIdentifier);
        return new Piece(mp, mp, false, pieceClassifier, Constants.BlackQueenIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard king.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the king belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard king.</returns>
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
            return new Piece(mp, mp, true, pieceClassifier, Constants.WhiteKingIdentifier);
        return new Piece(mp, mp, true, pieceClassifier, Constants.BlackKingIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard knight.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the knight belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard knight.</returns>
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
            return new Piece(mp, mp, false, pieceClassifier,Constants.WhiteKnightIdentifier);
        return new Piece(mp, mp, false, pieceClassifier,Constants.BlackKnightIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard black pawn.
    /// </summary>
    /// <returns>an instance of Piece with the movement pattern of a standard black pawn.</returns>
    public static Piece BlackPawn()
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.South
        };
        var capturePattern = new List<Tuple<int,int>> {
            Constants.SouthEast,
            Constants.SouthWest
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1)
        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        var cp = new RegularMovementPattern(capturePattern, moveLength);
        return new Piece(mp, cp, false, PieceClassifier.BLACK, Constants.BlackPawnIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard white pawn.
    /// </summary>
    /// <returns>an instance of Piece with the movement pattern of a standard white pawn.</returns>
    public static Piece WhitePawn()
    {
        var pattern = new List<Tuple<int,int>> {
            Constants.North
        };
        var capturePattern = new List<Tuple<int,int>> {
            Constants.NorthEast,
            Constants.NorthWest
        };
        var moveLength = new List<Tuple<int,int>> {
            new Tuple<int,int> (1,1),
            new Tuple<int,int> (1,1)

        };
        var mp = new RegularMovementPattern(pattern, moveLength);
        var cp = new RegularMovementPattern(capturePattern, moveLength);

        return new Piece(mp, cp, false, PieceClassifier.WHITE, Constants.WhitePawnIdentifier);
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