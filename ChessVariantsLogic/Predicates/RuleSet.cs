using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Predicates;
public class RuleSet
{
    private readonly IPredicate _moveRule;
    private readonly IPredicate _winRule;

    public RuleSet(IPredicate moveRule, IPredicate winRule)
    {
        this._moveRule = moveRule;
        this._winRule = winRule;
    }

    public ISet<string> applyMoveRule(Chessboard board, Player sideToPlay)
    {
        var possibleMoves = board.GetAllMoves(sideToPlay);
        var acceptedMoves = new HashSet<string>();
        foreach (var move in possibleMoves)
        {
            var futureBoard = board.CopyBoard();
            futureBoard.Move(move);
            bool ruleSatisfied = _moveRule.Evaluate(board, futureBoard);
            if (ruleSatisfied)
            {
                acceptedMoves.Add(move);
            }
        }
        return acceptedMoves;
    }

    public bool applyWinRule(Chessboard thisBoard)
    {
        return _winRule.Evaluate(thisBoard, thisBoard);
    }
}
