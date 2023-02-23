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


    /*
    Action
        {
            explosionAction,
            increaseScore,
            insertPiece
        }

    Event
    { IPredicate, Action}

    {
        Event stealPiece = Event(pieceCaptured, insertPiece)
        Event explodeOnCapture = Event(pieceCaptured, explosionAction);
        Event checkKing = Event(checked(king), increaseScore(1));


    }
    */
    
    public ISet<string> applyMoveRule(Chessboard board, string sideToPlay)
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

    public bool applyWinRule(Chessboard thisBoard, string sideToPlay)
    {
        var possibleMoves = thisBoard.GetAllMoves(sideToPlay);
        foreach (var move in possibleMoves)
        {
            var nextBoard = thisBoard.CopyBoard();
            nextBoard.Move(move);
            bool ruleSatisfied = _winRule.Evaluate(thisBoard, nextBoard);
            if (!ruleSatisfied)
            {
                return false;
            }
        }
        return true;
    }
}
