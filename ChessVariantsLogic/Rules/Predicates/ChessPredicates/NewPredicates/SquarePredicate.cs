using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates.NewPredicates;
public abstract class SquarePredicate : IPredicate
{
    [JsonProperty]
    private readonly BoardState _boardState;
    [JsonProperty]
    private readonly RelativeTo _relativeTo;
    [JsonProperty]
    protected readonly IPosition _square;

    public SquarePredicate(BoardState boardState, RelativeTo relativeTo, IPosition square)
    {
        _boardState = boardState;
        _relativeTo = relativeTo;
        _square = square;
    }

    public abstract bool Evaluate(BoardTransition transition);

    protected MoveWorker GetBoardState(BoardTransition transition)
    {
        return _boardState == BoardState.THIS ? transition.ThisState : transition.NextState;
    }

    protected string GetRelativeTo(BoardTransition transition)
    {
        return _relativeTo == RelativeTo.FROM ? transition.MoveFrom : transition.MoveTo;
    }

    protected string? GetFinalPosition(BoardTransition transition)
    {
        var board = GetBoardState(transition);
        string relativePosition = GetRelativeTo(transition);
        return _square.GetPosition(board, relativePosition);
    }

}
public enum SquarePredType
{
    ATTACKED, HAS_MOVED, IS, PIECE_AT, HAS_RANK, HAS_FILE
}
