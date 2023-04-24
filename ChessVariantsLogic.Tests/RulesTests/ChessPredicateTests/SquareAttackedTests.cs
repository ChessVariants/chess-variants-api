using Xunit;
using System;
using System.Diagnostics;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

namespace ChessVariantsLogic.Tests;

public class SquareAttackedTests : IDisposable {
    private readonly MoveWorker board;
    private readonly MoveWorker whiteKingAttackedBoard;
    private readonly MoveWorker blackKingAttackedBoard;
    private readonly MoveWorker blackBishopOnE2;
    
    private readonly BoardTransition whiteKingAttackedTransition;
    private readonly BoardTransition blackBishopOnE2Transition;

    public SquareAttackedTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        whiteKingAttackedBoard = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        whiteKingAttackedBoard.Board.Insert(Constants.BlackQueenIdentifier, "e2");

        blackKingAttackedBoard = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        blackKingAttackedBoard.Board.Insert(Constants.WhiteRookIdentifier, "e7");

        blackBishopOnE2 = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        blackBishopOnE2.Board.Insert(Constants.BlackBishopIdentifier, "e2");

        whiteKingAttackedTransition = new BoardTransition(whiteKingAttackedBoard, new Move("a1a1", Piece.WhitePawn()));
        blackBishopOnE2Transition = new BoardTransition(blackBishopOnE2, new Move("a1a1", Piece.WhitePawn()));

    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void WhiteKingIsAttacked_ShouldReturnTrue()
    {
        IPredicate whiteKingAttacked = new SquareAttacked(new PositionAbsolute("e1"), BoardState.THIS, Player.Black);
        Assert.True(whiteKingAttacked.Evaluate(whiteKingAttackedTransition));
    }
    
    [Fact]
    public void WhiteKingIsAttacked_ShouldReturnFalse()
    {
        IPredicate whiteKingAttacked = new SquareAttacked(new PositionAbsolute("e1"), BoardState.THIS, Player.Black);
        Assert.False(whiteKingAttacked.Evaluate(blackBishopOnE2Transition));
    }
}