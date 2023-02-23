using ChessVariantsLogic;

internal class PieceCaptured : IPredicate
{
    string pieceIdentifier;

    public PieceCaptured(string pieceIdentifier)
    {
        this.pieceIdentifier = pieceIdentifier;
    }


    public bool evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        int amountOfPieces = FindPiecesOfType(thisBoardState, pieceIdentifier).Count();
        int amountOfPiecesNextState = FindPiecesOfType(nextBoardState, pieceIdentifier).Count();
        int diff = amountOfPiecesNextState - amountOfPieces;
        return diff > 0;
    }

    static string getPlayer(string pieceIdentifier)
    {
        return "black";
    }

    private static bool PieceChecked(Chessboard board, string pieceIdentifier)
    {
        string player = getPlayer(pieceIdentifier);
        string attacker = player == "white" ? "black" : "white";
        var piecePositions = FindPiecesOfType(board, pieceIdentifier);
        foreach (var attackerMove in board.GetAllMoves(attacker))
        {
            var (_, to) = board.parseMove(attackerMove);
            if (piecePositions.Contains(to))
            {
                return true;
            }
        }
        return false;
    }

    private static IEnumerable<string> FindPiecesOfType(Chessboard board, string pieceIdentifier)
    {
        var pieceLocations = new List<string>();
        foreach (var position in board.CoorToIndex.Keys)
        {
            if (IsOfType(position, board, pieceIdentifier))
            {
                pieceLocations.Add(position);
            }
        }
        return pieceLocations;
    }

    private static bool IsOfType(string position, Chessboard board, string pieceIdentifier)
    {
        var piece = board.GetPiece(position);
        switch (pieceIdentifier)
        {
            // TODO "ANY_BLACK", "ANY_WHITE" instead
            case "ANY":
                return piece != Constants.UnoccupiedSquareIdentifier;
            case "ROYAL":
                return piece == Constants.BlackKingIdentifier;
            default:
                return piece == pieceIdentifier;
        }
    }

}