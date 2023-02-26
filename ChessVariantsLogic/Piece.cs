namespace ChessVariantsLogic;

/// <summary>
/// Represents a piece with a fixed movement pattern.
/// </summary>
public class Piece
{
    private readonly MovementPattern movementPattern;
    private bool royal;
    private PieceClassifier pieceClassifier;
    private bool hasMoved;

    public Piece(MovementPattern movementPattern, bool royal, PieceClassifier pc, bool hasMoved)
    {
        this.movementPattern = movementPattern;
        this.royal = royal;
        this.pieceClassifier = pc;
        this.hasMoved = hasMoved;
    }

    public Piece(MovementPattern movementPattern, bool royal, PieceClassifier pc)
    {
        this.movementPattern = movementPattern;
        this.royal = royal;
        this.pieceClassifier = pc;
        this.hasMoved = false;
    }

    public MovementPattern MovementPattern
    {
        get { return this.movementPattern; }
    }

    public bool Royal
    {
        get { return this.royal; }
    }

    public PieceClassifier PieceClassifier
    {
        get { return this.PieceClassifier;}
    }

    public static bool canTake(Piece p1, Piece p2)
    {
        if(p1.pieceClassifier == PieceClassifier.WHITE && p2.pieceClassifier == PieceClassifier.BLACK)
        {
            return true;
        }
        else if(p1.pieceClassifier == PieceClassifier.BLACK && p2.pieceClassifier == PieceClassifier.WHITE)
        {
            return true;
        }        
        else
        {
            return false;
        }
    }

    public static Piece Rook(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.North,
            MovementPattern.East,
            MovementPattern.South,
            MovementPattern.West
        };
        var mp = new MovementPattern(pattern, false, (0, Constants.MaxBoardHeigth), 0);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece Bishop(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.NorthEast,
            MovementPattern.SouthEast,
            MovementPattern.SouthWest,
            MovementPattern.NorthWest
        };
        var mp = new MovementPattern(pattern, false, (1, Constants.MaxBoardHeigth), 0);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece Queen(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.North,
            MovementPattern.East,
            MovementPattern.South,
            MovementPattern.West,
            MovementPattern.NorthEast,
            MovementPattern.SouthEast,
            MovementPattern.SouthWest,
            MovementPattern.NorthWest
        };
        var mp = new MovementPattern(pattern, false, (1,Constants.MaxBoardHeigth), 0);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece King(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.North,
            MovementPattern.East,
            MovementPattern.South,
            MovementPattern.West,
            MovementPattern.NorthEast,
            MovementPattern.SouthEast,
            MovementPattern.SouthWest,
            MovementPattern.NorthWest
        };
        var mp = new MovementPattern(pattern, false, (1,1), 0);
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
        var mp = new MovementPattern(pattern, true, (1, Constants.MaxBoardHeigth), 0);
        return new Piece(mp, false, pieceClassifier);
    }

    public static Piece BlackPawn()
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.South
        };
        var mp = new MovementPattern(pattern, false, (1, 1), 0);
        return new Piece(mp, false, PieceClassifier.BLACK);
    }

    public static Piece WhitePawn()
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.North
        };
        var mp = new MovementPattern(pattern, false, (1, 1), 0);
        return new Piece(mp, false, PieceClassifier.WHITE);
    }
    
}