namespace ChessVariantsLogic.Predicates;

/// <summary>
/// This class represents an accumulation of rules defined as <see cref="IPredicate"/>s, separated into move and win rules.
/// </summary>
public class RuleSet
{
    private readonly IPredicate _moveRule;
    private readonly IPredicate _winRule;
    private readonly IEnumerable<CustomMove> _customMoves;

    public RuleSet(IPredicate moveRule, IPredicate winRule, IEnumerable<CustomMove> customMoves)
    {
        _moveRule = moveRule;
        _winRule = winRule;
        _customMoves = customMoves;
    }

    /// <summary>
    /// From the set of all moves unconditionally possible; calculates the subset of those moves which are accepted by the internal moveRule.
    /// </summary>
    /// <param name="board">The current board state</param>
    /// <param name="sideToPlay">Which side is to make a move</param>
    /// <returns>All moves accepted by the game's moveRule</returns>
    public ISet<string> ApplyMoveRule(IBoardState board, Player sideToPlay)
    {
        var possibleMoves = board.GetAllValidMoves(sideToPlay);
        var acceptedMoves = new HashSet<string>();
        foreach (var move in possibleMoves)
        {
            var futureBoard = board.CopyBoardState();
            futureBoard.Move(move);
            bool ruleSatisfied = _moveRule.Evaluate(board, futureBoard);
            if (ruleSatisfied)
            {
                acceptedMoves.Add(move);
            }
        }

        foreach(var move in _customMoves)
        {
            IPredicate predicate = move.Predicate;
            IEnumerable<IAction> actions = move.Actions;
            string fromTo = move.From + move.To;

            var futureBoard = board.CopyBoardState();

            futureBoard.Move(fromTo);


            if (predicate.Evaluate(board, futureBoard))
            {
                acceptedMoves.Add(fromTo);
            }
        }

        return acceptedMoves;
    }

    /// <summary>
    /// WIP
    /// </summary>
    /// <param name="thisBoard"></param>
    /// <returns></returns>
    public bool ApplyWinRule(IBoardState thisBoard)
    {
        return _winRule.Evaluate(thisBoard, thisBoard);
    }
}
