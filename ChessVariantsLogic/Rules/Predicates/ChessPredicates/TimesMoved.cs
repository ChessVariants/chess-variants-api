namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// This predicate compares how many times a piece at a given location has moved compared to a specified _compareValue.
/// This is WIP and is not yet implemented.
/// </summary>
public class TimesMoved : IPredicate
{
    private readonly int _compareValue;
    private readonly Comparator _comparator;
    private readonly IPosition _position;


    public TimesMoved(IPosition position, Comparator comparator, int compareValue)
    {
        _position = position;
        _comparator = comparator;
        _compareValue = compareValue;
    }
    public TimesMoved(string coordinate, Comparator comparator, int compareValue)
    {

    }

    public bool Evaluate(BoardTransition transition)
    {
        throw new NotImplementedException();
    }
    private bool CompareValue(int timesMoved)
    {
        return _comparator switch
        {
            Comparator.GREATER_THAN => timesMoved > _compareValue,
            Comparator.LESS_THAN => timesMoved < _compareValue,
            Comparator.LESS_THAN_OR_EQUALS => timesMoved <= _compareValue,
            Comparator.GREATER_THAN_OR_EQUALS => timesMoved >= _compareValue,
            Comparator.EQUALS => timesMoved == _compareValue,
            Comparator.NOT_EQUALS => timesMoved != _compareValue,
            _ => false,
        };
    }
}
