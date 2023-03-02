using Xunit;
using ChessVariantsLogic.Predicates;
using System;
using System.Diagnostics;

namespace ChessVariantsLogic.Tests;

public class AttackedTests : IDisposable {
    IBoardState board;
    IBoardState whiteKingAttackedBoard;
    IBoardState blackKingAttackedBoard;
    IBoardState blackBishopOnE2;

    public AttackedTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        whiteKingAttackedBoard = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        whiteKingAttackedBoard.Board.Insert(Constants.BlackQueenIdentifier, "e2");

        blackKingAttackedBoard = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        blackKingAttackedBoard.Board.Insert(Constants.WhiteRookIdentifier, "e7");

        blackBishopOnE2 = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        blackBishopOnE2.Board.Insert(Constants.BlackBishopIdentifier, "e2");


    }

    public void Dispose()
    {
        board = new MoveWorker(Chessboard.StandardChessboard());
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void WhitePawnIsAttacked_ShouldReturnTrue()
    {
        IPredicate whiteKingAttacked = new Attacked(BoardState.THIS, Constants.WhitePawnIdentifier);
        Assert.True(whiteKingAttacked.Evaluate(whiteKingAttackedBoard, whiteKingAttackedBoard));
    }
    
    [Fact]
    public void WhitePawnIsAttacked_ShouldReturnFalse()
    {
        IPredicate whiteKingAttacked = new Attacked(BoardState.THIS, Constants.WhitePawnIdentifier);
        Assert.False(whiteKingAttacked.Evaluate(blackBishopOnE2, blackBishopOnE2));
    }

    [Fact]
    public void WhiteRoyalIsAttacked_ShouldReturnTrue()
    {
        IPredicate whiteKingAttacked = new Attacked(BoardState.THIS, "ROYAL_WHITE");
        Assert.True(whiteKingAttacked.Evaluate(whiteKingAttackedBoard, whiteKingAttackedBoard));
    }
    [Fact]
    public void WhiteRoyalIsAttacked_ShouldReturnFalse()
    {
        IPredicate whiteRoyalAttacked = new Attacked(BoardState.THIS, "ROYAL_WHITE");
        Assert.False(whiteRoyalAttacked.Evaluate(board, board));
    }

    [Fact]
    public void BlackRoyalIsAttacked_ShouldReturnTrue()
    {
        IPredicate blackKingAttacked = new Attacked(BoardState.THIS, "ROYAL_BLACK");
        Assert.True(blackKingAttacked.Evaluate(blackKingAttackedBoard, whiteKingAttackedBoard));
    }
    [Fact]
    public void BlackRoyalIsAttacked_ShouldReturnFalse()
    {
        IPredicate whiteRoyalAttacked = new Attacked(BoardState.THIS, "ROYAL_BLACK");
        Assert.False(whiteRoyalAttacked.Evaluate(board, board));
    }

    [Fact]
    public void AnyWhitePieceAttacked_ShouldReturnTrue()
    {
        IPredicate anyWhiteAttacked = new Attacked(BoardState.THIS, "ANY_WHITE");
        Assert.True(anyWhiteAttacked.Evaluate(blackBishopOnE2, blackBishopOnE2));
    }

    [Fact]
    public void AnyWhitePieceAttacked_ShouldReturnFalse()
    {
        IPredicate anyWhiteAttacked = new Attacked(BoardState.THIS, "ANY_WHITE");
        Assert.False(anyWhiteAttacked.Evaluate(board, board));
    }

    [Fact]
    public void AnyPieceAttacked_ShouldReturnTrue()
    {
        IPredicate anyAttacked = new Attacked(BoardState.THIS, "ANY");
        Assert.True(anyAttacked.Evaluate(blackBishopOnE2, blackBishopOnE2));
    }

    [Fact]
    public void AnyPieceAttacked_ShouldReturnFalse()
    {
        IPredicate anyAttacked = new Attacked(BoardState.THIS, "ANY");
        Assert.False(anyAttacked.Evaluate(board, board));
    }

    [Fact]
    public void AnyBlackPieceAttacked_ShouldReturnTrue()
    {
        IPredicate anyBlackAttacked = new Attacked(BoardState.THIS, "ANY_BLACK");
        Assert.True(anyBlackAttacked.Evaluate(blackBishopOnE2, blackBishopOnE2));
    }
    [Fact]
    public void AnyBlackPieceAttacked_ShouldReturnFalse()
    {
        IPredicate anyBlackAttacked = new Attacked(BoardState.THIS, "ANY_BLACK");
        Assert.False(anyBlackAttacked.Evaluate(board, board));
    }
}