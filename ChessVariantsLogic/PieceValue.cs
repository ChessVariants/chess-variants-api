using ChessVariantsLogic;


public class PieceValue
{
    private Dictionary<string, int> pieceValue;
   

    public PieceValue(List<Piece> pieces)
    {
        pieceValue = initStandardPieceValues();
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

    public int getValue(string piece)
    {
        return pieceValue[piece];
    }
}