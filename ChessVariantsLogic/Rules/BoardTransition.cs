using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Rules;
/// <summary>
/// Represents a board transition from one state to another.
/// The _moveFrom and _moveTo variables should be equal to the position of the piece performing the transition, before and after the transition.
/// </summary>
public class BoardTransition
{
    public readonly MoveWorker ThisState;
    public readonly MoveWorker NextState;
    public readonly Move Move;
    public readonly GameEvent Result;
    public readonly string MoveFrom;
    public readonly string MoveTo;

    public BoardTransition(MoveWorker thisState, Move move)
    {
        ThisState = thisState;
        NextState = thisState.CopyBoardState();
        Move = move;
        Result = Move.Perform(NextState);

        Tuple<string, string>? fromTo = MoveWorker.ParseMove(move.FromTo);
        if (fromTo == null) throw new ArgumentException("The given move parameter does not contain a proper move string. Supplied move string: " + move.FromTo);

        MoveFrom = fromTo.Item1;
        MoveTo = fromTo.Item2;
    }
    
    public bool IsValid()
    {
        return Result != GameEvent.InvalidMove;
    }

}
