using Xunit;
using System;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Tests;

public class FirstMoveTests
{
    MoveWorker board;
    BoardTransition boardTransition0;
    BoardTransition boardTransition1;
    string fromStr;
    string toStr0;
    string toStr1;

    public FirstMoveTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        fromStr = "e2";
        toStr0 = "e3";
        toStr1 = "e4";
        Move move0 = new Move(fromStr + toStr0, PieceClassifier.WHITE);
        Move move1 = new Move(toStr0 + toStr1, PieceClassifier.WHITE);

        boardTransition0 = new BoardTransition(board, move0);
        boardTransition1 = new BoardTransition(boardTransition0.NextState, move1);
    }

    [Fact]
    public void FirstMove_ShouldReturnTrue()
    {
        IPredicate firstMove = new FirstMove();
        Assert.True(firstMove.Evaluate(boardTransition0));
    }
    [Fact]
    public void FirstMove_ShouldReturnFalse()
    {
        IPredicate firstMove = new FirstMove();
        Assert.False(firstMove.Evaluate(boardTransition1));
    }
}