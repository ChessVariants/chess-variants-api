using System;
using Xunit;

namespace ChessVariantsLogic.Tests;

public class ChessboardTests
{

    /// <summary>
    /// Tests that the FEN representation of the board is of the correct format.
    /// </summary>
    [Fact]
    public void Test_FEN()
    {
        var gameDriver = new GameDriver(new Chessboard(6));

        Assert.Equal("6/6/6/6/6/6", gameDriver.Board.ReadBoardAsFEN());

        gameDriver.Board = new Chessboard(12, 3);
        Assert.Equal("3/3/3/3/3/3/3/3/3/3/3/3", gameDriver.Board.ReadBoardAsFEN());

        gameDriver.Board.Insert(Constants.BlackBishopIdentifier, "b2");
        Assert.Equal("3/3/3/3/3/3/3/3/3/3/1b1/3", gameDriver.Board.ReadBoardAsFEN());

        gameDriver.Board = Chessboard.StandardChessboard();
        Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", gameDriver.Board.ReadBoardAsFEN());

        gameDriver.Move("a2a4");
        Assert.Equal("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR", gameDriver.Board.ReadBoardAsFEN());

    }

    /// <summary>
    /// Tests that the standard chessboard is set up correctly.
    /// </summary>
    [Fact]
    public void Test_Standard_Chessboard()
    {
        var board = Chessboard.StandardChessboard();
        Assert.Equal(Constants.WhiteKingIdentifier,         board.GetPieceAsString("e1"));
        Assert.Equal(Constants.BlackQueenIdentifier,        board.GetPieceAsString("d8"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  board.GetPieceAsString("e4"));
        Assert.Equal(Constants.BlackRookIdentifier,         board.GetPieceAsString("a8"));
    }

    /// <summary>
    /// Tests that moving a piece updates the board correctly.
    /// </summary>
    [Fact]
    public void Test_Move_Piece()
    {
        var gameDriver = new GameDriver(Chessboard.StandardChessboard());

        Assert.Equal(Constants.WhitePawnIdentifier,         gameDriver.Board.GetPieceAsString("g2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("g3"));
        Assert.True(gameDriver.Move("g2g3"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("g2"));
        Assert.Equal(Constants.WhitePawnIdentifier,         gameDriver.Board.GetPieceAsString("g3"));
        
        Assert.False(gameDriver.Move("h2h9"));
        Assert.Equal(Constants.WhitePawnIdentifier, gameDriver.Board.GetPieceAsString("h2"));

        
    }

    /// <summary>
    /// Tests that pieces can not be inserted into squares outside of the chessboard.
    /// </summary>
    [Fact]
    public void Test_Faulty_Indices()
    {
        var board = Chessboard.StandardChessboard();
        Assert.False(board.Insert(Constants.BlackBishopIdentifier, 2, 9));
        Assert.False(board.Insert(Constants.BlackBishopIdentifier, 9, 9));
        Assert.False(board.Insert(Constants.BlackBishopIdentifier, 10, 7));
        Assert.True(board.Insert(Constants.BlackBishopIdentifier, "h8"));

        board = new Chessboard(12, 12);
        Assert.True(board.Insert(Constants.WhiteBishopIdentifier, "j3"));
        Assert.False(board.Insert(Constants.WhiteBishopIdentifier, "n3"));
        Assert.False(board.Insert(Constants.WhiteBishopIdentifier, new Tuple<int, int>(3, 14)));

    }

}
