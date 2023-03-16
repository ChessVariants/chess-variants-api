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
        var possibleMoves = transition._nextState.GetAllValidMoves(_player);
        foreach (var moveCoordinates in possibleMoves)
        {
            Move move = new Move(moveCoordinates);
            BoardTransition newTransition = new BoardTransition(transition._nextState, move);
            bool ruleSatisfied = _rule.Evaluate(newTransition);
            if (!ruleSatisfied)
            {
                return false;
            }
        }
        return true;

    }
}