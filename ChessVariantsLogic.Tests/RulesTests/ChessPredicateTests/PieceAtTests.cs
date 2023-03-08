using Xunit;
using System;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using ChessVariantsLogic.Rules;

namespace ChessVariantsLogic.Tests;

public class PieceAtTests : IDisposable {
    MoveWorker board;
    BoardTransition boardTransition;

    public PieceAtTests()
    {
        board = new MoveWorker(Chessboard.StandardChessboard());
        boardTransition = new BoardTransition(board, board, "a1a1");
    }

    public void Dispose()
    {
        board = new MoveWorker(Chessboard.StandardChessboard());
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void WhiteKingAtE1_ShouldReturnTrue()
    {
        IPredicate kingAtE1 = new PieceAt(Constants.WhiteKingIdentifier, Tuple.Create(7, 4), BoardState.THIS);
        Assert.True(kingAtE1.Evaluate(boardTransition));
    }
    [Fact]
    public void WhiteKingAtE2_ShouldReturnFalse()
    {
        IPredicate kingAtE2 = new PieceAt(Constants.WhiteKingIdentifier, Tuple.Create(6, 4), BoardState.THIS);
        Assert.False(kingAtE2.Evaluate(boardTransition));
    }
    [Fact]
    public void UnoccupiedSquareOnE4_ShouldReturnTrue()
    {
        IPredicate unoccupiedSquareOnE4 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(3, 4), BoardState.THIS);
        Assert.False(unoccupiedSquareOnE4.Evaluate(boardTransition));
    }
    [Fact]
    public void UnoccupiedSquareOnE1_ShouldReturnFalse()
    {
        IPredicate unoccupiedSquareOnE4 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(7, 4), BoardState.THIS);
        Assert.False(unoccupiedSquareOnE4.Evaluate(boardTransition));
    }
}