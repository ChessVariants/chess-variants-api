using ChessVariantsLogic;

internal class PieceCaptured : IPredicate
{
    string pieceIdentifier;
    string player;

    public PieceCaptured(string pieceIdentifier, string player)
    {
        this.pieceIdentifier = pieceIdentifier;
        this.player = player;
    }


    public bool evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        int amountOfPieces = FindPiecesOfType(thisBoardState, player, pieceIdentifier).Count();
        int amountOfPiecesNextState = FindPiecesOfType(nextBoardState, player, pieceIdentifier).Count();
        int diff = amountOfPiecesNextState - amountOfPieces;
        return diff > 0;
    }

    private static IEnumerable<string> FindPiecesOfType(Chessboard board, string player, string pieceIdentifier)
    {
        var pieceLocations = new List<string>();
        foreach (var position in board.CoorToIndex.Keys)
        {
            if (IsOfType(position, board, player, pieceIdentifier))
            {
                pieceLocations.Add(position);
            }
        }
        return pieceLocations;
    }

    private static bool IsOfType(string position, Chessboard board, string player, string pieceIdentifier)
    {
        var piece = board.GetPiece(position);
        switch (pieceIdentifier)
        {
            case "ANY":
                return piece != Constants.UnoccupiedSquareIdentifier;
            case "ROYAL":
                {
                    if (player == "black")
                    {
                        return piece == Constants.BlackKingIdentifier;
                    }
                    return piece == Constants.WhiteKingIdentifier;
                }
            default: return piece == pieceIdentifier;
        }
    }

}