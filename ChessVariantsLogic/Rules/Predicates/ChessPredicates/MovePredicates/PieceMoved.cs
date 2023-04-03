using ChessVariantsLogic.Rules.Moves;
using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate determines if a piece with the internal _pieceIdentifier was moved during the board transition.
/// </summary>
public class PieceMoved : MovePredicate
{
    [JsonProperty]
    private readonly string _pieceIdentifier;

    public PieceMoved(string pieceIdentifier, MoveState moveState = MoveState.THIS) : base(moveState)
    {
        _pieceIdentifier = pieceIdentifier;
    }


    public override bool Evaluate(BoardTransition transition)
    {
        Move? move = GetMove(transition);
        if(move == null) return false;
        var piece = move.Piece;

        return Utils.IsOfType(piece, _pieceIdentifier);
    }

}

public enum MoveState
{
    THIS, LAST
}