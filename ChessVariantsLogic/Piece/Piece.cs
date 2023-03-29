using ChessVariantsLogic.Export;

namespace ChessVariantsLogic;

/// <summary>
/// Represents a piece with a fixed movement pattern.
/// </summary>
public class Piece
{
#region Fields, properties and constructors
    private readonly MovementPattern movementPattern;
    private readonly MovementPattern capturePattern;
    private readonly bool royal;
    private readonly bool canBeCaptured;
    private readonly PieceClassifier pieceClassifier;

    private string pieceIdentifier;

    private readonly int repeat;

    public bool Royal { get { return this.royal; } }

    public PieceClassifier PieceClassifier { get { return this.pieceClassifier;} }

    public int Repeat { get { return this.repeat; } }

    public string PieceIdentifier { get { return this.pieceIdentifier; } }

    public bool CanBeCaptured { get { return this.canBeCaptured; } }

    /// <summary>
    /// Constructor for a new Piece.
    /// </summary>
    /// <param name="movementPattern">is the custom movement pattern of the type MovementPattern</param>
    /// <param name="capturePattern">is the custom capture pattern of the type MovementPattern</param>
    /// <param name="royal">set true if the piece is royal</param>
    /// <param name="pc">is the player the piece belongs to</param>
    /// <param name="repeat">is the amount of times the movement pattern can be repeated on the same turn</param>
    /// <param name="pieceIdentifier">is the unique string representation of the piece</param>
    public Piece(MovementPattern movementPattern, MovementPattern capturePattern, bool royal, PieceClassifier pc, int repeat, string pieceIdentifier, bool canBeCaptured)
    {
        this.movementPattern = movementPattern;
        this.capturePattern = capturePattern;
        this.royal = royal;
        this.pieceClassifier = pc;
        this.pieceIdentifier = pieceIdentifier;
        this.canBeCaptured = canBeCaptured;

        //This might not be optimal since it doesn't notify the user that the value is not what it was set to.
        if(repeat < 0)
            this.repeat = 0;
        else if (repeat > 3)
            this.repeat = 3;
        else
            this.repeat = repeat;
    }

    public Piece(MovementPattern movementPattern, MovementPattern capturePattern, bool royal, PieceClassifier pc, string pieceIdentifier, bool canBeCaptured = true)
    : this(movementPattern, capturePattern, royal, pc, 0, pieceIdentifier, canBeCaptured) {}
    
    public static Piece ParseState(PieceState state)
    {
        var movement = new MovementPattern(fetchPatterns(state.Movement));
        var captures = new MovementPattern(fetchPatterns(state.Captures));

        return new Piece(movement, captures, state.Royal, state.PieceClassifier, state.Repeat, state.PieceIdentifier, state.CanBeCaptured);
    }

    private static List<IPattern> fetchPatterns(List<Pattern> patterns)
    {
        var movementPatterns = new List<IPattern>();
        foreach(var p in patterns)
        {
            IPattern pattern;
            if(p.MinLength <= 0)
                pattern = new JumpPattern(p.XDir, p.YDir);
            else
                pattern = new RegularPattern(p.XDir, p.YDir, p.MinLength, p.MaxLength);
            movementPatterns.Add(pattern);
        }
        return movementPatterns;
    }

#endregion

    public string ExportAsJson()
    {
        return PieceExporter.ExportPieceStateAsJson(this);
    }

    /// <summary>
    /// Gets a specific movement pattern by index.
    /// </summary>
    /// <param name="index" is the index of the movement pattern></param>
    /// <returns>the movement pattern at <paramref name="index"/> if the index is valid, otherwise null.</returns>
    public IPattern? GetMovementPattern(int index)
    {
        return this.movementPattern.GetPattern(index);
    }

    /// <summary>
    /// Gets a specific capture pattern by index.
    /// </summary>
    /// <param name="index" is the index of the capture pattern></param>
    /// <returns>the capture pattern at <paramref name="index"/> if the index is valid, otherwise null.</returns>
    public IPattern? GetCapturePattern(int index)
    {
        return this.capturePattern.GetPattern(index);
    }

    /// <summary>
    /// Yield returns all IPatterns existing in this movement pattern.
    /// </summary>
    /// <returns>each IPattern in this movement pattern individually.</returns>
    public IEnumerable<IPattern> GetAllMovementPatterns()
    {
        return this.movementPattern.GetAllPatterns();
    }

    /// <summary>
    /// Yield returns all IPatterns existing in this capture pattern.
    /// </summary>
    /// <returns>each IPattern in this capture pattern individually.</returns>
    public IEnumerable<IPattern> GetAllCapturePatterns()
    {
        return this.capturePattern.GetAllPatterns();
    }

    /// <summary>
    /// Checks that this and <paramref name="other"/> are of opposite colors and that <paramref name="other"/> can be captured.
    /// </summary>
    /// <param name="other"> is the other piece</param>
    /// <returns> true if this is of opposite color than other, otherwise false.</returns>
    public bool CanTake(Piece other)
    {
        return !this.pieceClassifier.Equals(other.pieceClassifier) && other.CanBeCaptured;
    }

#region Static methods

    /// <summary>
    /// Creates a Piece object that behaves like a standard rook.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the rook belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard rook.</returns>
    public static Piece Rook(PieceClassifier pieceClassifier)
    {
        var patterns = new List<IPattern> {
            new RegularPattern(Constants.North, 1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.East,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.South, 1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.West,  1, Constants.MaxBoardHeight)
        };
        var mp = new MovementPattern(patterns);

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
        var patterns = new List<IPattern> {
            new RegularPattern(Constants.NorthEast,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.SouthEast,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.SouthWest,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.NorthWest,  1, Constants.MaxBoardHeight)
        };
        var mp = new MovementPattern(patterns);

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
        var patterns = new List<IPattern> {
            new RegularPattern(Constants.North,     1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.NorthEast, 1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.East,      1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.SouthEast, 1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.South,     1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.SouthWest, 1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.West,      1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.NorthWest, 1, Constants.MaxBoardHeight)
        };
        var mp = new MovementPattern(patterns);

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
        var patterns = new List<IPattern> {
            new RegularPattern(Constants.North,     1, 1),
            new RegularPattern(Constants.NorthEast, 1, 1),
            new RegularPattern(Constants.East,      1, 1),
            new RegularPattern(Constants.SouthEast, 1, 1),
            new RegularPattern(Constants.South,     1, 1),
            new RegularPattern(Constants.SouthWest, 1, 1),
            new RegularPattern(Constants.West,      1, 1),
            new RegularPattern(Constants.NorthWest, 1, 1)
        };
        var mp = new MovementPattern(patterns);
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
        var pattern = new List<IPattern> {
            new JumpPattern( 1, 2),
            new JumpPattern( 2, 1),
            new JumpPattern( 1,-2),
            new JumpPattern( 2,-1),
            new JumpPattern(-1, 2),
            new JumpPattern(-2, 1),
            new JumpPattern(-1,-2),
            new JumpPattern(-2,-1),
        };
        var mp = new MovementPattern(pattern);
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
        var capturePatterns = new List<IPattern> {
            new RegularPattern(Constants.SouthEast, 1, 1),
            new RegularPattern(Constants.SouthWest, 1, 1)
        };
        var patterns = new List<IPattern> {
            new RegularPattern(Constants.South, 1,1),
        };
        var mp = new MovementPattern(patterns);
        var cp = new MovementPattern(capturePatterns);
        return new Piece(mp, cp, false, PieceClassifier.BLACK, Constants.BlackPawnIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard white pawn.
    /// </summary>
    /// <returns>an instance of Piece with the movement pattern of a standard white pawn.</returns>
    public static Piece WhitePawn()
    {
        var capturePatterns = new List<IPattern> {
            new RegularPattern(Constants.NorthEast, 1, 1),
            new RegularPattern(Constants.NorthWest, 1, 1)
        };
        var patterns = new List<IPattern> {
            new RegularPattern(Constants.North, 1,1),
        };
        var mp = new MovementPattern(patterns);
        var cp = new MovementPattern(capturePatterns);
        return new Piece(mp, cp, false, PieceClassifier.WHITE, Constants.WhitePawnIdentifier);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a duck.
    /// </summary>
    /// <returns>an instance of Piece with the movement pattern of a duck.</returns>
    public static Piece Duck()
    {
        var capturePatterns = new List<IPattern> {
        };
        var patterns = new List<IPattern> {
        };

        for (int i = -Constants.MaxBoardWidth; i < Constants.MaxBoardWidth; i++)
        {
            for (int j = -Constants.MaxBoardHeight; j < Constants.MaxBoardHeight; j++)
            {
                if (i == 0 && j == 0) continue;
                patterns.Add(new JumpPattern(i, j));
            }
        }

        var mp = new MovementPattern(patterns);
        var cp = new MovementPattern(capturePatterns);
        return new Piece(mp, cp, false, PieceClassifier.SHARED, Constants.DuckIdentifier, false);
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

    /// <summary>
    /// Instantiates a HashSet of all the standard pieces and the duck piece.
    /// </summary>
    /// <returns>A HashSet containing all the standard pieces and the duck piece.</returns>
    public static HashSet<Piece> AllDuckChessPieces()
    {
        HashSet<Piece> pieces = AllStandardPieces();
        pieces.Add(Duck());
        return pieces;
    }

    #endregion

}