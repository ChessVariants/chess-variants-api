using System;
namespace ChessVariantsLogic.Rules;

public class CheckRule : Rule
{
    public ISet<string> applyRule(Chessboard currentBoard, Chessboard boardCopy, string player)
    {
        string opponent = player == "black" ? "white" : "black";
        var acceptedMoves = new HashSet<string>();

        foreach (var playerMove in currentBoard.GetAllMoves(player))
        {
            boardCopy.Move(playerMove);
            var royalPositions = FindRoyalPositions(boardCopy, player);
            bool royalStillAttacked = CheckIfOpponentAttacksKing(boardCopy, royalPositions, opponent);
            if (!royalStillAttacked)
            {
                acceptedMoves.Add(playerMove);
            }
            // boardCopy.ReverseLastestMove();
        };
        return acceptedMoves;
    }

    private static bool CheckIfOpponentAttacksKing(Chessboard board, IEnumerable<string> newRoyalPositions, string attacker)
    {
        foreach (var attackerMove in board.GetAllMoves(attacker))
        {
            var (from, to) = board.parseMove(attackerMove);
            if (newRoyalPositions.Contains(to))
            {
                return true;
            }
        }
        return false;
    }

    private static IEnumerable<string> FindRoyalPositions(Chessboard board, string player)
    {
        List<String> royalLocations = new List<String>(); ;
        foreach (var position in board.CoorToIndex.Keys)
        {
            if (IsRoyal(position, board, player))
            {
                royalLocations.Add(position);
            }
        }
        return royalLocations;
    }

    private static bool IsRoyal(string position, Chessboard board, string player)
    {
        var piece = board.GetPiece(position);
        if (player == "black")
        {
            return piece == Constants.BlackKingIdentifier;
        }
        return piece == Constants.WhiteKingIdentifier;
    }
}