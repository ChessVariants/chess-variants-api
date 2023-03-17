namespace ChessVariantsLogic;

public static class Constants
{
    public const int MaxBoardWidth = 20;
    public const int MaxBoardHeigth = 20;

    public const string BoardFiles = "abcdefghijklmnopqrst";

    //public const Piece BlackRook = new Piece(movement, false, moveLength, 0, false, false, PieceClassifier.BLACK);

#region Square identifiers
    public const string UnoccupiedSquareIdentifier = "--";
    public const string BlackRookIdentifier = "ro";
    public const string BlackKnightIdentifier = "kn";
    public const string BlackBishopIdentifier = "bi";
    public const string BlackQueenIdentifier = "qu";
    public const string BlackKingIdentifier = "ki";
    public const string BlackPawnIdentifier = "pa";
    public const string WhiteRookIdentifier = "RO";
    public const string WhiteKnightIdentifier = "KN";
    public const string WhiteBishopIdentifier = "BI";
    public const string WhiteQueenIdentifier = "QU";
    public const string WhiteKingIdentifier = "KI";
    public const string WhitePawnIdentifier = "PA";
    public const string DuckIdentifier = "DU";
    #endregion

    #region Directions
    public static Tuple<int, int> North = new Tuple<int, int>(-1,0);
    public static Tuple<int, int> East = new Tuple<int, int>(0,1);
    public static Tuple<int, int> South = new Tuple<int, int>(1,0);
    public static Tuple<int, int> West = new Tuple<int, int>(0,-1);
    public static Tuple<int, int> NorthEast = new Tuple<int, int>(-1,1);
    public static Tuple<int, int> SouthEast = new Tuple<int, int>(1,1);
    public static Tuple<int, int> SouthWest = new Tuple<int, int>(1,-1);
    public static Tuple<int, int> NorthWest = new Tuple<int, int>(-1,-1);
#endregion

}