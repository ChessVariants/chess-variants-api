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
    public readonly ISet<GameEvent> Results;
    public readonly string MoveFrom;
    public readonly string MoveTo;


    public BoardTransition(MoveWorker thisState, Move move)
    {
        ThisState = thisState.CopyBoardState();
        NextState = thisState.CopyBoardState();
        Move = move;
        Results = Move.Perform(NextState);
        Tuple<string, string>? fromTo = MoveWorker.ParseMove(move.FromTo);
        if (fromTo == null) throw new ArgumentException("The given move parameter does not contain a proper move string. Supplied move string: " + move.FromTo);

        MoveFrom = fromTo.Item1;
        MoveTo = fromTo.Item2;

    }

    public BoardTransition(MoveWorker thisState, Move move, ISet<Event> events) : this(thisState, move)
    {
        if (!IsValid())
            return;
        
        BoardTransition tmp = new BoardTransition(thisState, move);

        foreach (Event e in events)
        {
            if(e.ShouldRun(tmp))
            {
                Results.UnionWith(e.Run(NextState, move));
            }
        }
    }

    public bool IsValid()
    {
        return !Results.Contains(GameEvent.InvalidMove);
    }

}
