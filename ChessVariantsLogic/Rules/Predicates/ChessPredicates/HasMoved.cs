namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
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
        throw new NotImplementedException();
    }
}
