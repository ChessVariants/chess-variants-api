namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// This predicate evaluates if the last move is equal to the compare values.
/// </summary>
public class FirstMove : IPredicate
{

    public bool Evaluate(BoardTransition transition)
    {
        return transition.ThisState.getLastMove() == null;
    }
}
