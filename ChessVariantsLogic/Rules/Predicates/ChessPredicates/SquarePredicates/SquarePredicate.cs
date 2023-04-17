using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
/// <summary>
/// A predicate that evaluates if something is true about a certain square on the board.
/// /// Can either evaluate this state or the next state.
/// </summary>
public abstract class SquarePredicate : IPredicate
{
    [JsonProperty]
    private readonly BoardState _boardState;
    [JsonProperty]
    protected readonly IPosition _square;

    public SquarePredicate(BoardState boardState, IPosition square)
    {
        _boardState = boardState;
        _square = square;
    }

    public abstract bool Evaluate(BoardTransition transition);

    protected MoveWorker GetBoardState(BoardTransition transition)
    {
        return _boardState == BoardState.THIS ? transition.ThisState : transition.NextState;
    }

    protected string? GetFinalPosition(BoardTransition transition)
    {
        var board = GetBoardState(transition);
        return _square.GetPosition(board, transition.MoveFromTo);
    }

}