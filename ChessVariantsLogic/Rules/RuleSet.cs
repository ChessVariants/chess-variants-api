using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules;

/// <summary>
/// This class represents an accumulation of rules defined as <see cref="IPredicate"/>s, separated into move and win rules.
/// </summary>
public class RuleSet
{
    private readonly IPredicate _moveRule;
    private readonly IPredicate _winRule;
    private readonly ISet<MoveData> _customMoves;

    public RuleSet(IPredicate moveRule, IPredicate winRule, ISet<MoveData> customMoves)
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
    public IEnumerable<Move> ApplyMoveRule(IBoardState board, Player sideToPlay)
    {
        var possibleMoves = board.GetAllValidMoves(sideToPlay);
        var acceptedMoves = new List<Move>();
        foreach (var move in possibleMoves)
        {
            var futureBoard = board.CopyBoardState();
            futureBoard.Move(move);

            BoardTransition transition = new BoardTransition(board, futureBoard, move);

            bool ruleSatisfied = _moveRule.Evaluate(transition);
            if (ruleSatisfied)
            {
                acceptedMoves.Add(new MoveStandard(move));
            }
        }

        foreach (var moveData in _customMoves)
        {
            acceptedMoves.AddRange(moveData.GetValidMoves(board, _moveRule));
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
        return _winRule.Evaluate(new BoardTransition(thisBoard, thisBoard, ""));
    }
}
