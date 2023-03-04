using ChessVariantsLogic.Actions;

namespace ChessVariantsLogic.Predicates;

/// <summary>
/// This class represents an accumulation of rules defined as <see cref="IPredicate"/>s, separated into move and win rules.
/// </summary>
public class RuleSet
{
    private readonly IPredicate _moveRule;
    private readonly IPredicate _winRule;
    private readonly ISet<Move> _whiteCustomMoves;
    private readonly ISet<Move> _blackCustomMoves;

    public RuleSet(IPredicate moveRule, IPredicate winRule, ISet<Move> whiteCustomMoves, ISet<Move> blackCustomMoves)
    {
        _moveRule = moveRule;
        _winRule = winRule;
        _whiteCustomMoves = whiteCustomMoves;
        _blackCustomMoves = blackCustomMoves;
    }

    /// <summary>
    /// From the set of all moves unconditionally possible; calculates the subset of those moves which are accepted by the internal moveRule.
    /// </summary>
    /// <param name="board">The current board state</param>
    /// <param name="sideToPlay">Which side is to make a move</param>
    /// <returns>All moves accepted by the game's moveRule</returns>
    public ISet<Move> ApplyMoveRule(IBoardState board, Player sideToPlay)
    {
        var possibleMoves = board.GetAllValidMoves(sideToPlay);
        var acceptedMoves = new HashSet<Move>();
        foreach (var move in possibleMoves)
        {
            var futureBoard = board.CopyBoardState();
            futureBoard.Move(move);
            bool ruleSatisfied = _moveRule.Evaluate(board, futureBoard);
            if (ruleSatisfied)
            {
                acceptedMoves.Add(new MoveStandard(move));
            }
        }
        ISet<Move> customMoves;

        if (sideToPlay == Player.White)
            customMoves = _whiteCustomMoves;
        else if (sideToPlay == Player.Black)
            customMoves = _blackCustomMoves;
        else
            return acceptedMoves;

        foreach(var move in customMoves)
        {
            var futureBoard = board.CopyBoardState();
            move.Perform(futureBoard);
            if (move.EvaluatePredicate(board, futureBoard))
            {
                acceptedMoves.Add(move);
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
