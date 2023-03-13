namespace ChessVariantsLogic.Predicates;

using static ChessVariantsLogic.Game;

/// <summary>
/// This class represents an accumulation of rules defined as <see cref="IPredicate"/>s, separated into move and win rules.
/// </summary>
public class RuleSet
{
    private readonly IPredicate _moveRule;
    private readonly IPredicate _winRule;

    public RuleSet(IPredicate moveRule, IPredicate winRule)
    {
        _moveRule = moveRule;
        _winRule = winRule;
    }

    public Dictionary<string, List<string>> GetLegalMoveDict(Player player, IBoardState board)
    {
        var moveDict = new Dictionary<string, List<string>>();
        var moves = ApplyMoveRule(board, player);
        foreach (var move in moves)
        {
            var fromTo = board.parseMove(move);
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


        return acceptedMoves;
    }

    /// <summary>
    /// Using the moves generated from ApplyMoveRule, this method checks if the opponent can make a move. If not, the game is won.
    /// </summary>
    /// <param name="board">The current board state</param>
    /// <param name="playerTurn">Side that made last move a move</param>
    /// <returns>true if game is one for the side that just made a move</returns>
    public bool ApplyWinRule(IBoardState board, Player playerTurn)
    {
        Player opponent = playerTurn == Player.White ? Player.Black : Player.White;
        var acceptedMoves = ApplyMoveRule(board, opponent);
        foreach (var move in acceptedMoves)
        {
            var futureBoard = board.CopyBoardState();
            futureBoard.Move(move);
            bool ruleSatisfied = _winRule.Evaluate(board, futureBoard);
            if (!ruleSatisfied)
            {
                return false;
            }
        }
        return true;
    }
}
