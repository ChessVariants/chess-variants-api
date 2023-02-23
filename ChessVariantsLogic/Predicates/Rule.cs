using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Predicates;
internal class Rule
{
    private IPredicate moveRule;
    private IPredicate winRule;
    private WinRuleType winRuleType;

    public Rule(IPredicate moveRule, IPredicate winRule, WinRuleType winRuleType)
    {
        this.moveRule = moveRule;
        this.winRule = winRule;
        this.winRuleType = winRuleType;
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
    void test()
    {
        IPredicate whiteChessWinRule = new Checked(BoardState.NEXT, Constants.BlackKingIdentifier, "black");
        IPredicate whiteChessMoveRule = new Function(FunctionType.NOT, new Checked(BoardState.NEXT, Constants.WhiteKingIdentifier, "white"));


        IPredicate whiteAntiChessMoveRule = new Operator(PredType.IMPLIES, new Checked(BoardState.THIS, "ANY", "black"), new PieceCaptured("ANY", "black"));
        IPredicate whiteAntiChessWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, "ANY", "black");


        IPredicate whiteCaptureTheKingMoveRule = new Function(FunctionType.TRUE, null);
        IPredicate whiteCaptureTheKingWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, Constants.BlackKingIdentifier, "black");

        IPredicate blackWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, Constants.WhitePawnIdentifier, "white");


        Rule classicalChess = new Rule(whiteChessMoveRule, whiteChessWinRule, WinRuleType.MUST_SATISFY_ALL);

        Rule antiChess = new Rule(whiteAntiChessMoveRule, whiteAntiChessWinRule, WinRuleType.MUST_SATISFY_NEXT);

        Rule captureTheKing = new Rule(whiteCaptureTheKingMoveRule, whiteCaptureTheKingWinRule, WinRuleType.MUST_SATISFY_NEXT);

    }


    public ISet<string> applyMoveRule(Chessboard board, string sideToPlay)
    {
        var possibleMoves = board.GetAllMoves(sideToPlay);
        var acceptedMoves = new HashSet<string>();
        foreach (var move in possibleMoves)
        {
            var futureBoard = board.CopyBoard();
            futureBoard.Move(move);
            bool ruleSatisfied = moveRule.evaluate(board, futureBoard);
            if (ruleSatisfied)
            {
                acceptedMoves.Add(move);
            }
        }
        return acceptedMoves;
    }

    public bool applyWinRule(Chessboard thisBoard, Chessboard nextBoard, string sideToPlay)
    {
        switch(winRuleType)
        {
            case WinRuleType.MUST_SATISFY_ALL:
            {
                var opponent = sideToPlay == "black" ? "white" : "black";
                var possibleMoves = nextBoard.GetAllMoves(opponent);
                foreach (var move in possibleMoves)
                {
                    var futureBoard = nextBoard.CopyBoard();
                    futureBoard.Move(move);
                    bool ruleSatisfied = winRule.evaluate(nextBoard, futureBoard);
                    if (!ruleSatisfied)
                    {
                        return false;
                    }
                }
                return true;
            }
            case WinRuleType.MUST_SATISFY_NEXT:
                return winRule.evaluate(thisBoard, nextBoard);
            default:
                return false;
        }
    }



}

public enum WinRuleType
{
    MUST_SATISFY_NEXT, MUST_SATISFY_ALL
}
