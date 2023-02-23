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

        // Some examples of different predicates and rules that can be constructed from these classes
        // Each predicate evaluates a transition from one board state to another


        // CAPTURE THE KING

        // Since checkmate isn't an issue, any move is valid. (Predicate will always return true)
        IPredicate whiteCaptureTheKingMoveRule = new Function(FunctionType.TRUE, null);
        // White wins if the amount of black kings left is equal to 0
        IPredicate whiteCaptureTheKingWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, Constants.BlackKingIdentifier);

        Rule captureTheKing = new Rule(whiteCaptureTheKingMoveRule, whiteCaptureTheKingWinRule, WinRuleType.MUST_SATISFY_NEXT);

        // CLASSICAL CHESS

        // This one is a bit tricky since it needs to hold for all possible next board states :/
        IPredicate blackKingIsCheckedDuringThisTurn = new Checked(BoardState.THIS, Constants.BlackKingIdentifier);
        IPredicate blackKingIsCheckedDuringNextTurn = new Checked(BoardState.NEXT, Constants.BlackKingIdentifier);
        IPredicate whiteChessWinRule = new Operator(PredType.AND, blackKingIsCheckedDuringThisTurn, blackKingIsCheckedDuringNextTurn);
        // White move is possible if white king is not checked during next board state
        IPredicate whiteChessMoveRule = new Function(FunctionType.NOT, new Checked(BoardState.NEXT, Constants.WhiteKingIdentifier));

        // ANTI CHESS

        // White move is possible if any black piece being checked during this state implies that a piece is captured during the transition
        IPredicate whiteAntiChessMoveRule = new Operator(PredType.IMPLIES, new Checked(BoardState.THIS, "ANY_BLACK"), new PieceCaptured("ANY_BLACK"));
        // White wins if the amount of white pieces left is equal to 0
        IPredicate whiteAntiChessWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, "ANY_WHITE");

        // HORDE

        // If black has captured all white's pawns, black wins.
        IPredicate blackHordeWinRule = new PiecesLeft(Comparator.EQUALS, 0, BoardState.NEXT, Constants.WhitePawnIdentifier);

        // Since checkmate isn't an issue, any move is valid. (Predicate will always return true)
        IPredicate blackHordeMoveRule = new Function(FunctionType.TRUE, null);


        Rule classicalChessRuleWhite = new Rule(whiteChessMoveRule, whiteChessWinRule, WinRuleType.MUST_SATISFY_ALL);

        Rule antiChessRuleWhite = new Rule(whiteAntiChessMoveRule, whiteAntiChessWinRule, WinRuleType.MUST_SATISFY_NEXT);


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
