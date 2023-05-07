using ChessVariantsLogic;
//using static ChessVariantsLogic.Piece;

public class PieceValue
{
    private Dictionary<string, double> _pieceValue;
    private double _jumpPatternValue = 2.1;
    private int _regularPatternValue = 1;
    private HashSet<Piece> _pieces;
    private Chessboard _chessboard;


    public PieceValue(HashSet<Piece> Pieces, Chessboard chessboard)
    {
        _pieces = Pieces;
        _chessboard = chessboard;
        _pieceValue = initPieces();
    }
    public Dictionary<string, double> initStandardPieceValues()
    {
        var dictionary = new Dictionary<string, double>();

        dictionary.Add(Constants.WhitePawnIdentifier, 1);
        dictionary.Add(Constants.WhiteKnightIdentifier, 3);
        dictionary.Add(Constants.WhiteBishopIdentifier, 3);
        dictionary.Add(Constants.WhiteRookIdentifier, 5);
        dictionary.Add(Constants.WhiteQueenIdentifier, 9);
        dictionary.Add(Constants.WhiteKingIdentifier, 0);

        dictionary.Add(Constants.BlackPawnIdentifier, -1);
        dictionary.Add(Constants.BlackKnightIdentifier, -3);
        dictionary.Add(Constants.BlackBishopIdentifier, -3);
        dictionary.Add(Constants.BlackRookIdentifier, -5);
        dictionary.Add(Constants.BlackQueenIdentifier, -9);
        dictionary.Add(Constants.BlackKingIdentifier, 0);

        dictionary.Add(Constants.UnoccupiedSquareIdentifier, 0);

        return dictionary;
    }

    public Dictionary<string, double> initPieces()
    {
        var dictionary = new Dictionary<string, double>();

        dictionary.Add(Constants.UnoccupiedSquareIdentifier, 0);

        foreach (var piece in _pieces)
        {
            double pieceValue = calculateMovementValue(piece) + calculateCaptureValue(piece);
            dictionary.Add(piece.PieceIdentifier, pieceValue);
        }

        return dictionary;
    }

    public double getValue(string piece)
    {
        return _pieceValue[piece];
    }

    public string GetHighestValuePieceIdentifier(ISet<string> promotablePieces, Player forPlayer)
    {
        var promotableEntires = _pieceValue.Where(pv => promotablePieces.Contains(pv.Key));
        if (forPlayer == Player.White)
        {
            return promotableEntires.OrderByDescending(pv => pv.Value).First().Key;
        }
        return 
        promotableEntires.OrderBy(pv => pv.Value).First().Key;
    }

    private double calculateMovementValue(Piece piece)
    {
        double value = 0;
        int maxRow;
        int maxCol;
        foreach (var pattern in piece.GetAllMovementPatterns())
        {
            maxRow = Math.Min(_chessboard.Rows - 1,pattern.MaxLength);
            maxCol = Math.Min(_chessboard.Cols - 1,pattern.MaxLength);
            if (pattern is JumpPattern)
            {
                value += _jumpPatternValue;
            }
            else if (pattern is RegularPattern && pattern.XDir == 0)
            {
                value += (maxCol - pattern.MinLength + 1) * (piece.Repeat + 1);
            }
            else if (pattern is RegularPattern && pattern.YDir == 0)
            {
                value += (maxRow - pattern.MinLength + 1) * (piece.Repeat + 1);
            }
            else
            {
                int minBoard = Math.Min(maxCol, maxRow);
                int maxMoves = Math.Min(minBoard, pattern.MaxLength);
                value +=  ((maxMoves - pattern.MinLength + 1) * (piece.Repeat + 1)*16.8/28);
            }
            

        }
        if (piece.PieceClassifier.Equals(PieceClassifier.BLACK))
        {
            value = -value;
        }
        return value;
    }

    private double calculateCaptureValue(Piece piece)
    {
        double value = 0;
        int maxRow;
        int maxCol;
        foreach (var pattern in piece.GetAllCapturePatterns())
        {
            maxRow = Math.Min(_chessboard.Rows - 1,pattern.MaxLength);
            maxCol = Math.Min(_chessboard.Cols - 1,pattern.MaxLength);
            if (pattern is JumpPattern)
            {
                value += _jumpPatternValue;
            }
            else if (pattern is RegularPattern && pattern.XDir == 0)
            {
                value += (maxCol - pattern.MinLength + 1) * (piece.Repeat + 1);
            }
            else if (pattern is RegularPattern && pattern.YDir == 0)
            {
                value += (maxRow - pattern.MinLength + 1) * (piece.Repeat + 1);
            }
            else
            {
                int minBoard = Math.Min(maxCol, maxRow);
                int maxMoves = Math.Min(minBoard, pattern.MaxLength);
                value +=  ((maxMoves - pattern.MinLength + 1) * (piece.Repeat + 1)*16.8/28);
            }
        }
        if (piece.PieceClassifier.Equals(PieceClassifier.BLACK))
        {
            value = -value;
        }
        return value;
    }
}
