namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// This predicates evaluates a rule which has to hold for every possible move from the current board state.
/// </summary>
public class ForEvery : IPredicate
{
    private readonly IPredicate _rule;
    private readonly Player _player;

    public ForEvery(IPredicate rule, Player player)
    {
        _rule = rule;
        _player = player;
    }

    /// <summary>
    /// Evaluates to true if the internal rule holds for every possible move in the nextState of the supplied boardTransition state for the internal player, otherwise false.
    /// </summary>
    /// <param name="transition">The board transition to be evaluated.</param>
    /// <returns>True if the internal rule holds for every possible move in the nextState of the supplied boardTransition state for the internal player, otherwise false.</returns>
    public bool Evaluate(BoardTransition transition)
    {
        var possibleMoves = transition.NextState.GetAllValidMoves(_player);
        foreach (var moveCoordinates in possibleMoves)
        {
            var (from, _) = transition.NextState.ParseMove(moveCoordinates);

            var pieceIdentifier = transition.NextState.Board.GetPieceIdentifier(from);
            Move move = new Move(moveCoordinates, transition.NextState.GetPieceClassifier(pieceIdentifier));
            BoardTransition newTransition = new BoardTransition(transition.NextState, move);
            bool ruleSatisfied = _rule.Evaluate(newTransition);
            if (!ruleSatisfied)
            {
                return false;
            }
        }
        return true;

    }
}