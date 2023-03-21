namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// This predicate evaluates if this is the first move i.e the movelog is empy.
/// </summary>
public class FirstMove : IPredicate
{
    public bool Evaluate(BoardTransition transition)
    {
        return transition.ThisState.Movelog.Count == 0;
    }
}
