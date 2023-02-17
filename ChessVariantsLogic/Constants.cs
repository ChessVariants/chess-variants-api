namespace ChessVariantsLogic;

public static class Constants
{
    public const int MaxBoardWidth = 20;
    public const int MaxBoardHeigth = 20;

#region Square identifiers
    public const string UnoccupiedSquareIdentifier = "-";
    public const string BlackRookIdentifier = "r";
    public const string BlackKnightIdentifier = "n";
    public const string BlackBishopIdentifier = "b";
    public const string BlackQueenIdentifier = "q";
    public const string BlackKingIdentifier = "k";
    public const string BlackPawnIdentifier = "p";
    public const string WhiteRookIdentifier = "R";
    public const string WhiteKnightIdentifier = "N";
    public const string WhiteBishopIdentifier = "B";
    public const string WhiteQueenIdentifier = "Q";
    public const string WhiteKingIdentifier = "K";
    public const string WhitePawnIdentifier = "P";
#endregion

    // Maps square notation to corresponding index in Chessboard
    public static Dictionary<string, (int, int)> CoorToIndex = initDictionary();

    private static Dictionary<string, (int,int)> initDictionary()
    {
        var dictionary = new Dictionary<string, (int, int)>();

        string files = "abcdefghijklmnopqrst";

        for(int i = 0; i < MaxBoardHeigth; i++)
        {
            for(int j = 0; j < MaxBoardWidth; j++)
            {
                int rank = i+1;
                string notation = files[j] + rank.ToString();
                dictionary.Add(notation, (i,j));
            }
        }
        return dictionary;
    }

}