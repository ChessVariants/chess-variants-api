using ChessVariantsLogic;
//using static ChessVariantsLogic.Piece;

public class PieceValue
{
    private Dictionary<string, int> pieceValue;
    private int jumpPatternValue = 2;
    private int regularPatternValue = 1;
    private HashSet<Piece> pieces;
    private Chessboard _chessboard;


    public PieceValue(HashSet<Piece> Pieces, Chessboard chessboard)
    {
        pieces = Pieces;
        _chessboard = chessboard;
        pieceValue = initPieces();
    }
    public Dictionary<string, int> initStandardPieceValues()
    {
        var dictionary = new Dictionary<string, int>();

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

    public Dictionary<string, int> initPieces()
    {
        var dictionary = new Dictionary<string, int>();

        dictionary.Add(Constants.UnoccupiedSquareIdentifier, 0);

        foreach (var piece in pieces)
        {
            int pieceValue = calculateMovementValue(piece) + calculateCaptureValue(piece);
            dictionary.Add(piece.PieceIdentifier, pieceValue);
        }

        return dictionary;
    }

    public int getValue(string piece)
    {
        return pieceValue[piece];
    }

    private int calculateMovementValue(Piece piece)
    {
        int value = 0;
        int maxRow;
        int maxCol;;
        foreach (var pattern in piece.GetAllMovementPatterns())
        {
            maxRow = Math.Min(_chessboard.Rows,pattern.MaxLength);
            maxCol = Math.Min(_chessboard.Cols,pattern.MaxLength);
            if (pattern is JumpPattern)
            {
                value += jumpPatternValue;
            }
            else if (pattern is RegularPattern && pattern.XDir == 0)
            {
                value += (maxCol - pattern.MinLength) * (piece.Repeat + 1);
            }
            else if (pattern is RegularPattern && pattern.YDir == 0)
            {
                value += (maxRow - pattern.MinLength) * (piece.Repeat + 1);
            }
            else
            {
                int minBoard = Math.Min(maxCol, maxRow);
                int maxMoves = Math.Min(minBoard, pattern.MaxLength);
                value +=  ((maxCol - pattern.MinLength) * (piece.Repeat + 1)*10/14);
            }
            

        }
        if (piece.PieceClassifier.Equals(PieceClassifier.BLACK))
        {
            value = -value;
        }
        return value;
    }

    private int calculateCaptureValue(Piece piece)
    {
        int value = 0;
        
        foreach (var pattern in piece.GetAllCapturePatterns())
        {
            if (pattern is JumpPattern)
            {
                value += jumpPatternValue;
            }
            if (pattern is RegularPattern)
            {
                value += (pattern.MaxLength - pattern.MinLength + 1) * (piece.Repeat + 1);
            }
        }
        if (piece.PieceClassifier.Equals(PieceClassifier.BLACK))
        {
            value = -value;
        }
        return value;
    }
}