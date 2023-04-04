using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;
public abstract class PiecePredicate : IPredicate
{
    [JsonProperty]
    private readonly BoardState _boardState;
    [JsonProperty]
    protected readonly string _pieceIdentifier;

    public PiecePredicate(BoardState boardState, string pieceIdentifier)
    {
        _boardState = boardState;
        _pieceIdentifier = pieceIdentifier;
    }

    public abstract bool Evaluate(BoardTransition transition);

    protected MoveWorker GetBoardState(BoardTransition transition)
    {
        return _boardState == BoardState.THIS ? transition.ThisState : transition.NextState;
    }

}
