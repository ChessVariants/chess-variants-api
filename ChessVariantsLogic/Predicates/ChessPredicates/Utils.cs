using ChessVariantsLogic;
namespace ChessVariantsLogic.Predicates;

public class Utils {

    public static bool PieceAttacked(Chessboard board, string pieceIdentifier)
    {
        Player player = getPlayer(pieceIdentifier);
        Player attacker = player == Player.White ? Player.Black : Player.White;
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

    public static Player getPlayer(string pieceIdentifier)
    {
        if (pieceIdentifier.Any(char.IsUpper))
        {
            return Player.White;
        }
        else if (pieceIdentifier.Any(char.IsLower)) {
            return Player.Black;
        }
        return Player.None;
    }

    public static IEnumerable<string> FindPiecesOfType(Chessboard board, string pieceIdentifier)
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

    public static bool IsOfType(string position, Chessboard board, string pieceIdentifier)
    {
        var piece = board.GetPiece(position);
        switch (pieceIdentifier)
        {
            case "ANY":
                return piece != Constants.UnoccupiedSquareIdentifier;
            case "ANY_BLACK":
                if (piece != null) 
                {
                    return getPlayer(piece) == Player.Black;
                }
                return false;
            case "ANY_WHITE":
                if (piece != null) 
                {
                    return getPlayer(piece) == Player.White;
                }
                return false;
            case "ROYAL_BLACK":
                return piece == Constants.BlackKingIdentifier;
            case "ROYAL_WHITE":
                return piece == Constants.WhiteKingIdentifier;
            default:
                return piece == pieceIdentifier;
        }
    }
}