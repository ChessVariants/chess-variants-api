using Xunit;
using ChessVariantsLogic.Predicates;
using System;

namespace ChessVariantsLogic.Tests;

public class PiecesLeftTests : IDisposable {
    Chessboard board;

    public PiecesLeftTests()
    {
        board = Chessboard.StandardChessboard();
    }

    public void Dispose()
    {
        board = Chessboard.StandardChessboard();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void StandardChessOneWhiteKing_ShouldReturnTrue()
    {
        IPredicate oneWhiteKing = new PiecesLeft(Constants.WhiteKingIdentifier, Comparator.EQUALS, 1, BoardState.THIS);
        Assert.True(oneWhiteKing.Evaluate(board, board));
    }

    [Fact]
    public void StandardChessNotTwoWhiteKings_ShouldReturnTrue()
    {
        IPredicate notTwoWhiteKing = new PiecesLeft(Constants.WhiteKingIdentifier, Comparator.NOT_EQUALS, 2, BoardState.THIS);
        Assert.True(notTwoWhiteKing.Evaluate(board, board));
    }

    [Fact]
    public void StandardChessEightPawns_ShouldReturnTrue()
    {
        IPredicate eqEightWhitePawns = new PiecesLeft(Constants.WhitePawnIdentifier, Comparator.EQUALS, 8, BoardState.THIS);
        IPredicate ltNineWhitePawns = new PiecesLeft(Constants.WhitePawnIdentifier, Comparator.LESS_THAN, 9, BoardState.THIS);
        IPredicate lteNineWhitePawns = new PiecesLeft(Constants.WhitePawnIdentifier, Comparator.LESS_THAN_OR_EQUALS, 9, BoardState.THIS);
        IPredicate lteEightWhitePawns = new PiecesLeft(Constants.WhitePawnIdentifier, Comparator.LESS_THAN_OR_EQUALS, 8, BoardState.THIS);
        Assert.True(eqEightWhitePawns.Evaluate(board, board));
        Assert.True(ltNineWhitePawns.Evaluate(board, board));
        Assert.True(lteNineWhitePawns.Evaluate(board, board));
        Assert.True(lteEightWhitePawns.Evaluate(board, board));
    }

    [Fact]
    public void StandardChessThreeRooks_ShouldReturnFalse()
    {
        IPredicate eqThreeBlackRooks = new PiecesLeft(Constants.BlackRookIdentifier, Comparator.EQUALS, 3, BoardState.THIS);
        IPredicate gtThreeBlackRooks = new PiecesLeft(Constants.BlackRookIdentifier, Comparator.GREATER_THAN, 3, BoardState.THIS);
        IPredicate gteThreeBlackRooks = new PiecesLeft(Constants.BlackRookIdentifier, Comparator.GREATER_THAN_OR_EQUALS, 3, BoardState.THIS);
        Assert.False(eqThreeBlackRooks.Evaluate(board, board));
        Assert.False(gtThreeBlackRooks.Evaluate(board, board));
        Assert.False(gteThreeBlackRooks.Evaluate(board, board));
    }

    [Fact]
    public void StandardChessZeroUnknownPiece_ShouldReturnTrue()
    {
        IPredicate zeroUnknownPiece = new PiecesLeft("unknown_piece", Comparator.EQUALS, 0, BoardState.THIS);
        Assert.True(zeroUnknownPiece.Evaluate(board, board));
    }

    [Fact]
    public void StandardChessAny_ShouldReturnTrue()
    {
        IPredicate thirtyTwoNonEmptySquares = new PiecesLeft("ANY", Comparator.EQUALS, 32, BoardState.THIS);
        Assert.True(thirtyTwoNonEmptySquares.Evaluate(board, board));
    }


    [Fact]
    public void StandardChessAnyBlack_ShouldReturnTrue()
    {
        IPredicate sixteenBlackPieces = new PiecesLeft("ANY_BLACK", Comparator.EQUALS, 16, BoardState.THIS);
        Assert.True(sixteenBlackPieces.Evaluate(board, board));
    }

    [Fact]
    public void StandardChessAnyWhite_ShouldReturnTrue()
    {
        IPredicate sixteenWhitePieces = new PiecesLeft("ANY_WHITE", Comparator.EQUALS, 16, BoardState.THIS);
        Assert.True(sixteenWhitePieces.Evaluate(board, board));
    }
}