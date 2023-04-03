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
    public readonly ISet<GameEvent> Results;
    public readonly string MoveFrom;
    public readonly string MoveTo;


    public BoardTransition(MoveWorker thisState, Move move)
    {
        ThisState = thisState.CopyBoardState();
        NextState = thisState.CopyBoardState();
        Results = NextState.PerformMove(move);
        MoveFrom = move.From;
        MoveTo = move.To;
    }

    public BoardTransition(MoveWorker thisState, Move move, ISet<Event> events) : this(thisState, move)
    {
        foreach (var e in events)
        {
            if(e.ShouldRun(this))
            {
                NextState.RunEvent(e);
            }
        }
    }

    public bool IsValid()
    {
        return !Results.Contains(GameEvent.InvalidMove);
    }

}
