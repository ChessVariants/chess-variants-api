using Xunit;
using System;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Tests;

public class LastMoveClassifierTests
{
    MoveWorker board;
    BoardTransition boardTransition0;
    BoardTransition boardTransition1;
    BoardTransition boardTransition2;
    string fromStr0;
    string fromStr1;
    string toStr0;
    string toStr1;

    public LastMoveClassifierTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        fromStr0 = "e2";
        toStr0 = "e3";

        fromStr1 = "e7";
        toStr1 = "e6";

        Move move0 = new Move(fromStr0 + toStr0, PieceClassifier.WHITE);
        Move move1 = new Move(fromStr1 + toStr1, PieceClassifier.BLACK);

        boardTransition0 = new BoardTransition(board, move0);
        boardTransition1 = new BoardTransition(boardTransition0.NextState, move1);
        boardTransition2 = new BoardTransition(boardTransition1.NextState, move1);
    }

    [Fact]
    public void LastMoveWasWhite_ShouldReturnTrue()
    {
        IPredicate lastMoveWasWhite = new LastMoveClassifier(PieceClassifier.WHITE);
        Assert.True(lastMoveWasWhite.Evaluate(boardTransition1));
    }
    [Fact]
    public void LastMoveWasBlack_ShouldReturnFalse()
    {
        IPredicate lastMoveWasWhite = new LastMoveClassifier(PieceClassifier.BLACK);
        Assert.False(lastMoveWasWhite.Evaluate(boardTransition1));
    }


    [Fact]
    public void LastMoveWasBlack_ShouldReturnTrue()
    {
        IPredicate lastMoveWasWhite = new LastMoveClassifier(PieceClassifier.BLACK);
        Assert.True(lastMoveWasWhite.Evaluate(boardTransition2));
    }
    [Fact]
    public void LastMoveWasWhite_ShouldReturnFalse()
    {
        IPredicate lastMoveWasWhite = new LastMoveClassifier(PieceClassifier.WHITE);
        Assert.False(lastMoveWasWhite.Evaluate(boardTransition2));
    }
}