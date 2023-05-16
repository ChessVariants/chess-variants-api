using ChessVariantsLogic.Export;

namespace ChessVariantsLogic;

/// <summary>
/// Represents a piece with a fixed movement pattern.
/// </summary>
public class Piece
{
#region Fields, properties and constructors

    private MovementPattern movementPattern { get; }
    private MovementPattern capturePattern { get; }

    public PieceClassifier PieceClassifier { get; }

    public int Repeat { get; }

    public string PieceIdentifier { get; }

    public bool CanBeCaptured { get; }

    public bool CanBePromotedTo { get; }

    public string ImagePath { get; set; }

    /// <summary>
    /// Constructor for a new Piece.
    /// </summary>
    /// <param name="movementPattern">is the custom movement pattern of the type MovementPattern</param>
    /// <param name="capturePattern">is the custom capture pattern of the type MovementPattern</param>
    /// <param name="royal">set true if the piece is royal</param>
    /// <param name="pc">is the player the piece belongs to</param>
    /// <param name="repeat">is the amount of times the movement pattern can be repeated on the same turn</param>
    /// <param name="pieceIdentifier">is the unique string representation of the piece</param>
    /// <param name="imagePath">is the path to the image connected to the piece</param>
    public Piece(MovementPattern movementPattern, MovementPattern capturePattern, PieceClassifier pc, int repeat, string pieceIdentifier, bool canBeCaptured, bool canBePromotedTo, string imagePath)
    {
        this.movementPattern = movementPattern;
        this.capturePattern = capturePattern;
        this.PieceClassifier = pc;
        this.PieceIdentifier = pieceIdentifier;
        this.CanBeCaptured = canBeCaptured;
        this.CanBePromotedTo = canBePromotedTo;
        ImagePath = imagePath;

        //This might not be optimal since it doesn't notify the user that the value is not what it was set to.
        if(repeat < 0)
            this.Repeat = 0;
        else if (Repeat > 3)
            this.Repeat = 3;
        else
            this.Repeat = repeat;
    }

    public Piece(MovementPattern movementPattern, MovementPattern capturePattern, PieceClassifier pc, int repeat, string pieceIdentifier, bool canBeCaptured, bool canBePromotedTo)
    : this(movementPattern, capturePattern, pc, repeat, pieceIdentifier, canBeCaptured, canBePromotedTo, pieceIdentifier) {}

    public Piece(MovementPattern movementPattern, MovementPattern capturePattern, PieceClassifier pc, string pieceIdentifier, bool canBeCaptured = true, bool canBePromotedTo = true)
    : this(movementPattern, capturePattern, pc, 0, pieceIdentifier, canBeCaptured, canBePromotedTo) {}

    public Piece(MovementPattern movementPattern, MovementPattern capturePattern, PieceClassifier pc, string pieceIdentifier, string imagePath, bool canBeCaptured = true, bool canBePromotedTo = true)
    : this(movementPattern, capturePattern, pc, 0, pieceIdentifier, canBeCaptured, canBePromotedTo, imagePath) {}


#endregion

    /// <summary>
    /// Yield returns all IPatterns existing in this movement pattern.
    /// </summary>
    /// <returns>each IPattern in this movement pattern individually.</returns>
    public IEnumerable<Pattern> GetAllMovementPatterns()
    {
        return this.movementPattern.GetAllPatterns();
    }

    /// <summary>
    /// Yield returns all IPatterns existing in this capture pattern.
    /// </summary>
    /// <returns>each IPattern in this capture pattern individually.</returns>
    public IEnumerable<Pattern> GetAllCapturePatterns()
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
        return !this.PieceClassifier.Equals(other.PieceClassifier) && other.CanBeCaptured;
    }

#region Static methods

    private static HashSet<Pattern> fetchPatterns(List<PatternRecord> patterns)
    {
        var movementPatterns = new HashSet<Pattern>();
        foreach(var p in patterns)
        {
            Pattern pattern;
            if(p.MinLength <= 0)
                pattern = new JumpPattern(p.XDir, p.YDir);
            else
                pattern = new RegularPattern(p.XDir, p.YDir, p.MinLength, p.MaxLength);
            movementPatterns.Add(pattern);
        }
        return movementPatterns;
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard rook.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the rook belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard rook.</returns>
    public static Piece Rook(PieceClassifier pieceClassifier)
    {
        var patterns = new HashSet<Pattern> {
            new RegularPattern(Constants.North, 1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.East,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.South, 1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.West,  1, Constants.MaxBoardHeight)
        };
        var mp = new MovementPattern(patterns);

        if(pieceClassifier.Equals(PieceClassifier.WHITE))
            return new Piece(mp, mp, pieceClassifier, Constants.WhiteRookIdentifier, Constants.WhiteRookImage);
        return new Piece(mp, mp, pieceClassifier, Constants.BlackRookIdentifier, Constants.BlackRookImage);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard bishop.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the bishop belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard bishop.</returns>
    public static Piece Bishop(PieceClassifier pieceClassifier)
    {
        var patterns = new HashSet<Pattern> {
            new RegularPattern(Constants.NorthEast,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.SouthEast,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.SouthWest,  1, Constants.MaxBoardHeight),
            new RegularPattern(Constants.NorthWest,  1, Constants.MaxBoardHeight)
        };
        var mp = new MovementPattern(patterns);

        if(pieceClassifier.Equals(PieceClassifier.WHITE))
            return new Piece(mp, mp, pieceClassifier, Constants.WhiteBishopIdentifier, Constants.WhiteBishopImage);
        return new Piece(mp, mp, pieceClassifier, Constants.BlackBishopIdentifier, Constants.BlackBishopImage);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard queen.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the queen belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard queen.</returns>
    public static Piece Queen(PieceClassifier pieceClassifier)
    {
        var patterns = new HashSet<Pattern> {
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
            return new Piece(mp, mp, pieceClassifier, Constants.WhiteQueenIdentifier, Constants.WhiteQueenImage);
        return new Piece(mp, mp, pieceClassifier, Constants.BlackQueenIdentifier, Constants.BlackQueenImage);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard king.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the king belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard king.</returns>
    public static Piece King(PieceClassifier pieceClassifier)
    {
        var patterns = new HashSet<Pattern> {
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
            return new Piece(mp, mp, pieceClassifier, Constants.WhiteKingIdentifier, Constants.WhiteKingImage, canBePromotedTo: false);
        return new Piece(mp, mp, pieceClassifier, Constants.BlackKingIdentifier, Constants.BlackKingImage, canBePromotedTo: false);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard knight.
    /// </summary>
    /// <param name="pieceClassifier"> is the side that the knight belongs to</param>
    /// <returns> an instance of Piece with the movement pattern of a standard knight.</returns>
    public static Piece Knight(PieceClassifier pieceClassifier)
    {
        var pattern = new HashSet<Pattern> {
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
            return new Piece(mp, mp, pieceClassifier,Constants.WhiteKnightIdentifier, Constants.WhiteKnightImage);
        return new Piece(mp, mp, pieceClassifier,Constants.BlackKnightIdentifier, Constants.BlackKnightImage);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard black pawn.
    /// </summary>
    /// <returns>an instance of Piece with the movement pattern of a standard black pawn.</returns>
    public static Piece BlackPawn()
    {
        var capturePatterns = new HashSet<Pattern> {
            new RegularPattern(Constants.SouthEast, 1, 1),
            new RegularPattern(Constants.SouthWest, 1, 1)
        };
        var patterns = new HashSet<Pattern> {
            new RegularPattern(Constants.South, 1,1),
        };
        var mp = new MovementPattern(patterns);
        var cp = new MovementPattern(capturePatterns);
        return new Piece(mp, cp, PieceClassifier.BLACK, Constants.BlackPawnIdentifier, Constants.BlackPawnImage, canBePromotedTo: false);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a standard white pawn.
    /// </summary>
    /// <returns>an instance of Piece with the movement pattern of a standard white pawn.</returns>
    public static Piece WhitePawn()
    {
        var capturePatterns = new HashSet<Pattern> {
            new RegularPattern(Constants.NorthEast, 1, 1),
            new RegularPattern(Constants.NorthWest, 1, 1)
        };
        var patterns = new HashSet<Pattern> {
            new RegularPattern(Constants.North, 1,1),
        };
        var mp = new MovementPattern(patterns);
        var cp = new MovementPattern(capturePatterns);
        return new Piece(mp, cp, PieceClassifier.WHITE, Constants.WhitePawnIdentifier, Constants.WhitePawnImage, canBePromotedTo: false);
    }

    /// <summary>
    /// Creates a Piece object that behaves like a duck.
    /// </summary>
    /// <returns>an instance of Piece with the movement pattern of a duck.</returns>
    public static Piece Duck()
    {
        var capturePatterns = new HashSet<Pattern> {};
        var patterns = new HashSet<Pattern> {};

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
        return new Piece(mp, cp, PieceClassifier.SHARED, Constants.DuckIdentifier, "Du", false, canBePromotedTo: false);
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