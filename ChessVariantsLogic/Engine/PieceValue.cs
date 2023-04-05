using ChessVariantsLogic;
//using static ChessVariantsLogic.Piece;

public class PieceValue
{
    private Dictionary<string, int> pieceValue;
    private int jumpPatternValue = 3;
    private int regularPatternValue = 1;
    private HashSet<Piece> pieces;


    public PieceValue(HashSet<Piece> Pieces)
    {
        pieces = Pieces;
        pieceValue = initPieces();
    }

    public Dictionary<string, int> InitStandardPieceValues()
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
            int pieceValue = CalculateMovementValue(piece) + CalculateCaptureValue(piece);
            dictionary.Add(piece.PieceIdentifier, pieceValue);
        }

        return dictionary;
    }

    public int GetValue(string piece)
    {
        return pieceValue[piece];
    }

    private int CalculateMovementValue(Piece piece)
    {
        int value = 0;
        foreach (var pattern in piece.GetAllMovementPatterns())
        {
            if (pattern is JumpPattern)
            {
                value += jumpPatternValue;
            }
            if (pattern is RegularPattern)
            {
                value += (pattern.MaxLength - pattern.MinLength) * (piece.Repeat + 1);
            }
        }
        if (piece.PieceClassifier.Equals(PieceClassifier.BLACK))
        {
            value = -value;
        }
        return value;
    }

    private int CalculateCaptureValue(Piece piece)
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
