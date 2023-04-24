using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// A predicate counts a value and compares it to the internal _compareValue using the internal _comparator.
/// Can either evaluate this state or the next state.
/// </summary>
public abstract class CountPredicate : IPredicate
{
    [JsonProperty]
    private readonly Comparator _comparator;
    [JsonProperty]
    private readonly int _compareValue;
    [JsonProperty]
    private readonly BoardState _boardState;

    public CountPredicate(Comparator comparator, int compareValue, BoardState boardState)
    {
        _comparator = comparator;
        _compareValue = compareValue;
        _boardState = boardState;
    }

    public abstract bool Evaluate(BoardTransition transition);

    protected MoveWorker GetBoardState(BoardTransition transition)
    {
        return _boardState == BoardState.THIS ? transition.ThisState : transition.NextState;
    }

    protected bool CompareValue(int piecesLeft)
    {
        return _comparator switch
        {
            Comparator.GREATER_THAN => piecesLeft > _compareValue,
            Comparator.LESS_THAN => piecesLeft < _compareValue,
            Comparator.LESS_THAN_OR_EQUALS => piecesLeft <= _compareValue,
            Comparator.GREATER_THAN_OR_EQUALS => piecesLeft >= _compareValue,
            Comparator.EQUALS => piecesLeft == _compareValue,
            Comparator.NOT_EQUALS => piecesLeft != _compareValue,
            _ => false,
        };
    }
}

/// <summary>
/// Enum for the supported comparator types.
/// </summary>
public enum Comparator
{
    GREATER_THAN, LESS_THAN, GREATER_THAN_OR_EQUALS, LESS_THAN_OR_EQUALS, EQUALS, NOT_EQUALS
}