using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate determines if a piece is attacked or not, either in the current board state or the next.
/// </summary>
public class Attacked : PiecePredicate
{

    public Attacked(BoardState boardState, string pieceIdentifier) : base(boardState, pieceIdentifier)
    {
    }

    /// <summary>
    /// Evaluates to true/false if a piece is attacked or not in either the current board state or the next, depending on what was specified at object-creation.
    /// </summary>
    /// <inheritdoc/>
    /// <returns>true/false if a piece is attacked or not in either the current board state or the next, depending on what was specified at object-creation.</returns>
    public override bool Evaluate(BoardTransition transition)
    {
        var board = GetBoardState(transition);
        bool attacked;
        if (_pieceIdentifier == "ANY")
            attacked = Utils.PieceAttacked(board, "WHITE") || Utils.PieceAttacked(board, "BLACK");
        else
            attacked = Utils.PieceAttacked(board, _pieceIdentifier);
        return attacked;
    }

}
public enum BoardState
{
    THIS, NEXT
}