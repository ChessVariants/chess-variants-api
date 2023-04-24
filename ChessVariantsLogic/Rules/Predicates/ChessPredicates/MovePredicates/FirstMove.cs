namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate evaluates if this is the first move i.e the movelog is empy.
/// </summary>
public class FirstMove : MovePredicate
{
    public FirstMove(MoveState moveState = MoveState.THIS) :base(moveState)
    {

    }

    public override bool Evaluate(BoardTransition transition)
    {
        int moves = _moveState == MoveState.THIS ? 1 : 2;

        return transition.NextState.Movelog.Count == moves;
    }
}
