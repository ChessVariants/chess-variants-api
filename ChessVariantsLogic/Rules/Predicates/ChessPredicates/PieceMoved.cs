namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate determines if a piece with the internal _pieceIdentifier was moved during the board transition.
/// </summary>
public class PieceMoved : IPredicate
{
    private readonly string _pieceIdentifier;

    public PieceMoved(string pieceIdentifier)
    {
        _pieceIdentifier = pieceIdentifier;
    }

    
    public bool Evaluate(BoardTransition transition)
    {
        string piece = transition.ThisState.Board.GetPieceIdentifier(transition.MoveFrom);

        return (piece == _pieceIdentifier) && (transition.MoveFrom != transition.MoveTo);
    }

}