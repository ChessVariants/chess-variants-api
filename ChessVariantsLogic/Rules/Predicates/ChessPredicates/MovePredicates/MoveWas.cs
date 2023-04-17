using ChessVariantsLogic.Rules.Moves;
using Newtonsoft.Json;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// This predicate evaluates if the from and to values of a move is equal to the compare values.
/// </summary>
public class MoveWas : MovePredicate
{
    [JsonProperty]
    private readonly IPosition _compareFrom;
    [JsonProperty]
    private readonly IPosition _compareTo;

    public MoveWas(IPosition compareFrom, IPosition compareTo, MoveState moveState) : base(moveState)
    {
        _compareFrom = compareFrom;
        _compareTo = compareTo;
    }

    public override bool Evaluate(BoardTransition transition)
    {
        Move? move = GetMove(transition);
        if (move == null) return false;

        var moveCoordinates = MoveWorker.ParseMove(move.FromTo);
        if (moveCoordinates == null) return false;
        string? from = _compareFrom.GetPosition(transition.ThisState, transition.MoveFrom);
        string? to = _compareTo.GetPosition(transition.ThisState, transition.MoveFrom);

        if (from == null) return to == moveCoordinates.Item2;
        if (to == null) return from == moveCoordinates.Item1;

        string compareMove = from + to;
        return compareMove == moveCoordinates.Item1 + moveCoordinates.Item2;
    }
}
