using Xunit;
using System;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Tests;

public class MoveWasTests
{
    MoveWorker board;
    BoardTransition e2e3BoardTransition;
    BoardTransition e3e4BoardTransition;
    IPosition e2;
    IPosition e3;
    string fromStr;
    string toStr0;
    string toStr1;

    public MoveWasTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        fromStr = "e2";
        toStr0 = "e3";
        toStr1 = "e4";
        e2 = new PositionAbsolute(fromStr);
        e3 = new PositionAbsolute(toStr0);
        Move e2e3Move = new Move(fromStr + toStr0, Piece.WhitePawn());
        Move e3e4Move = new Move(toStr0 + toStr1, Piece.WhitePawn());

        e2e3BoardTransition = new BoardTransition(board, e2e3Move);
        e3e4BoardTransition = new BoardTransition(e2e3BoardTransition.NextState, e3e4Move);
    }

    [Fact]
    public void LastMoveWasE2E3_ShouldReturnTrue()
    {
        IPredicate lastMoveWasE2E3 = new MoveWas(e2, e3, MoveState.LAST);
        Assert.False(lastMoveWasE2E3.Evaluate(e2e3BoardTransition));
    }
    [Fact]
    public void LastMoveWasE2E3_ShouldReturnFalse()
    {
        IPredicate lastMoveWasE2E3 = new MoveWas(e2, e3, MoveState.LAST);
        Assert.True(lastMoveWasE2E3.Evaluate(e3e4BoardTransition));
    }
    [Fact]
    public void ThisMoveWasE2E3_ShouldReturnTrue()
    {
        IPredicate thisMoveWasE2E3 = new MoveWas(e2, e3, MoveState.THIS);
        Assert.True(thisMoveWasE2E3.Evaluate(e2e3BoardTransition));
    }
    [Fact]
    public void ThisMoveWasE2E3_ShouldReturnFalse()
    {
        IPredicate thisMoveWasE2E3 = new MoveWas(e2, e3, MoveState.THIS);
        Assert.False(thisMoveWasE2E3.Evaluate(e3e4BoardTransition));
    }
}