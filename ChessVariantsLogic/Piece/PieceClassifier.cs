namespace ChessVariantsLogic;

public enum PieceClassifier
{
    WHITE,
    BLACK,
    SHARED
}

public static class PieceClassifierExtension
{
    public static string AsString(this PieceClassifier pieceClassifier)
    {
        return pieceClassifier switch
        {
            PieceClassifier.WHITE => "white",
            PieceClassifier.BLACK => "black",
            PieceClassifier.SHARED => "shared",
            _ => throw new ArgumentException("Invalid PieceClassifier")
        };
    }
}