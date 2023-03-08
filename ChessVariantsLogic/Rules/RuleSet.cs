using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Moves.Actions;
using ChessVariantsLogic;
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

    public Dictionary<string, List<string>> GetLegalMoveDict(Player player, MoveWorker board)
    {
        var moveDict = new Dictionary<string, List<string>>();
        var moves = ApplyMoveRule(board, player);
        foreach (var move in moves)
        {
            var fromTo = board.parseMove(move.FromTo);
            if (fromTo == null)
            {
                throw new InvalidOperationException($"Could not parse move {move}");
            }

            var moveList = moveDict.GetValueOrDefault(fromTo.Item1, new List<string>());
            if (moveList.Count == 0)
            {
                moveDict[fromTo.Item1] = moveList;
            }
            moveList.Add(fromTo.Item2);

        }
        return moveDict;
    }


    /// <summary>
    /// From the set of all moves unconditionally possible; calculates the subset of those moves which are accepted by the internal moveRule.
    /// </summary>
    /// <param name="board">The current board state</param>
    /// <param name="sideToPlay">Which side is to make a move</param>
    /// <returns>All moves accepted by the game's moveRule</returns>
    public IEnumerable<SpecialMove> ApplyMoveRule(MoveWorker board, Player sideToPlay)
    {
        var possibleMoves = board.GetAllValidMoves(sideToPlay);
        var acceptedMoves = new List<SpecialMove>();
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
    public bool ApplyWinRule(MoveWorker thisBoard)
    {
        return _winRule.Evaluate(new BoardTransition(thisBoard, thisBoard, "a1a1"));
    }
}
