using Xunit;
using System;
using System.Diagnostics;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Tests;

public class PieceCapturedTests {
    MoveWorker board;

    BoardTransition pieceWasCapturedTransition;
    BoardTransition pieceWasNotCapturedTransition;

    public PieceCapturedTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());

        // (Scholars checkmate)

        board.Move("e2e3");
        board.Move("e3e4");
        board.Move("e7e6");
        board.Move("e6e5");

        board.Move("d1h5");
        board.Move("b8c6");

        board.Move("f1c4");
        board.Move("g8f6");

        pieceWasNotCapturedTransition = new BoardTransition(board, new Move("a2a3", PieceClassifier.WHITE));
        pieceWasCapturedTransition = new BoardTransition(board, new Move("h5f7", PieceClassifier.WHITE));
    }

    [Fact]
    public void PieceCaptured_ShouldReturnTrue()
    {
        IPredicate pawnCaptured = new PieceCaptured(Constants.BlackPawnIdentifier);
        IPredicate pieceCaptured = new PieceCaptured("ANY_BLACK");
        Assert.True(pawnCaptured.Evaluate(pieceWasCapturedTransition));
        Assert.True(pieceCaptured.Evaluate(pieceWasCapturedTransition));
    }
    [Fact]
    public void CheckMate_ShouldReturnFalse()
    {
        IPredicate pawnCaptured = new PieceCaptured(Constants.BlackPawnIdentifier);
        Assert.False(pawnCaptured.Evaluate(pieceWasNotCapturedTransition));
    }

}