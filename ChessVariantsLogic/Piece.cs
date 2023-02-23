namespace ChessVariantsLogic;

/// <summary>
/// Represents a piece with a fixed movement pattern.
/// </summary>
public class Piece
{
    private readonly List<Tuple<int, int>> movement;
    private bool jump;
    private (int, int) moveLength;
    private int repeat;
    private bool royal;
    private PieceClassifier pieceClassifier;
    private bool hasMoved;

    public Piece(List<Tuple<int, int>> movement, bool jump, (int,int) moveLength, int repeat, bool royal, bool hasMoved, PieceClassifier pc)
    {
        this.movement = movement;
        this.jump = jump;
        this.moveLength = moveLength;
        this.repeat = repeat;
        this.royal = royal;
        this.hasMoved = hasMoved;
        this.pieceClassifier = pc;
    }

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

    public bool Royal
    {
        get { return royal; }
    }

    public PieceClassifier PieceClassifier
    {
        get { return PieceClassifier;}
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
        return new Piece(pattern, false, (1, Constants.MaxBoardHeigth), 0, false, false, pieceClassifier);
    }

    public static Piece Bishop(PieceClassifier pieceClassifier)
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.NorthEast,
            MovementPattern.SouthEast,
            MovementPattern.SouthWest,
            MovementPattern.NorthWest
            };
        return new Piece(pattern, false, (1, Constants.MaxBoardHeigth), 0, false, false, pieceClassifier);
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
        return new Piece(pattern, false, (1, Constants.MaxBoardHeigth), 0, false, false, pieceClassifier);
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
        return new Piece(pattern, false, (1, 1), 0, true, false, pieceClassifier);
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
        
        return new Piece(pattern, true, (1, Constants.MaxBoardHeigth), 0, false, false, pieceClassifier);
    }

    public static Piece BlackPawn()
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.South
            };
        return new Piece(pattern, false, (1, 1), 0, false, false, PieceClassifier.BLACK);
    }

    public static Piece WhitePawn()
    {
        var pattern = new List<Tuple<int,int>> {
            MovementPattern.North
            };
        return new Piece(pattern, false, (1, 1), 0, false, false, PieceClassifier.WHITE);
    }
    
}