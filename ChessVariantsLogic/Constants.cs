namespace ChessVariantsLogic;

public static class Constants
{
    public const int MaxBoardWidth = 20;
    public const int MaxBoardHeigth = 20;

    public const string BoardFiles = "abcdefghijklmnopqrst";

    //public const Piece BlackRook = new Piece(movement, false, moveLength, 0, false, false, PieceClassifier.BLACK);

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