namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate evaluates if a a piece at the given position has moved or not.
/// This is WIP and is not yet implemented.
/// </summary>
public class HasMoved : IPredicate
{
    public HasMoved(Tuple<int, int> position, PositionType positionType)
    {

    }
    public HasMoved(string coordinate)
    {

    }

    public bool Evaluate(BoardTransition transition)
    {
        return false;
        //throw new NotImplementedException();
    }
}
