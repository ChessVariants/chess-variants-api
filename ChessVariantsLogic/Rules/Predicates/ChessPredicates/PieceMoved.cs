using ChessVariantsLogic.Rules.Moves;
using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate determines if a piece with the internal _pieceIdentifier was moved during the board transition.
/// </summary>
public class PieceMoved : IPredicate
{
    [JsonProperty]
    private readonly string _pieceIdentifier;
    [JsonProperty]
    private readonly MoveState _moveState;

    public PieceMoved(string pieceIdentifier, MoveState moveState = MoveState.THIS)
    {
        _pieceIdentifier = pieceIdentifier;
        _moveState = moveState;
    }

    
    public bool Evaluate(BoardTransition transition)
    {
        Piece piece = transition.Move.Piece;
        if (_moveState == MoveState.LAST)
        {
            Move? lastMove = transition.ThisState.GetLastMove();
            if (lastMove == null) return false;
            piece = lastMove.Piece;
        }

        if (_pieceIdentifier == "BLACK")
            return piece.PieceClassifier == PieceClassifier.BLACK;
        else if (_pieceIdentifier == "WHITE")
            return piece.PieceClassifier == PieceClassifier.WHITE;
        else if (_pieceIdentifier == "SHARED")
            return piece.PieceClassifier == PieceClassifier.SHARED;
        else
            return piece.PieceIdentifier == _pieceIdentifier;
    }

}

public enum MoveState
{
    THIS, LAST
}