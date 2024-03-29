namespace ChessVariantsLogic;

public static class Constants
{
    public const int MaxBoardWidth = 20;
    public const int MaxBoardHeight = 20;

    public const string BoardFiles = "abcdefghijklmnopqrst";

#region Square identifiers
    public const string UnoccupiedSquareIdentifier = "--";
    public const string BlackRookIdentifier = "BlackRook";
    public const string BlackKnightIdentifier = "BlackKnight";
    public const string BlackBishopIdentifier = "BlackBishop";
    public const string BlackQueenIdentifier = "BlackQueen";
    public const string BlackKingIdentifier = "BlackKing";
    public const string BlackPawnIdentifier = "BlackPawn";
    public const string WhiteRookIdentifier = "WhiteRook";
    public const string WhiteKnightIdentifier = "WhiteKnight";
    public const string WhiteBishopIdentifier = "WhiteBishop";
    public const string WhiteQueenIdentifier = "WhiteQueen";
    public const string WhiteKingIdentifier = "WhiteKing";
    public const string WhitePawnIdentifier = "WhitePawn";
    public const string DuckIdentifier = "DU";
    public const string SharedPieceIdentifier = "SH";

#endregion

#region Piece images

    public const string BlackRookImage = "ro";
    public const string BlackKnightImage = "kn";
    public const string BlackBishopImage = "bi";
    public const string BlackKingImage = "ki";
    public const string BlackQueenImage = "qu";
    public const string BlackPawnImage = "pa";
    public const string WhiteRookImage = "RO";
    public const string WhiteKnightImage = "KN";
    public const string WhiteBishopImage = "BI";
    public const string WhiteKingImage = "KI";
    public const string WhiteQueenImage = "QU";
    public const string WhitePawnImage = "PA";

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