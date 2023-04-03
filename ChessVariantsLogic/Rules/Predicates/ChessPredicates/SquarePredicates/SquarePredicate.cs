using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
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