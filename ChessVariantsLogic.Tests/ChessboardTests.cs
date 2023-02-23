using System;
using Xunit;

namespace ChessVariantsLogic.Tests;

/// <summary>
/// This class contains unit tests on Chessboard.cs and ChessDriver.cs.
/// </summary>
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
    public void Test_Move_Pawn()
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
    /// Test that a rook can move correcly on non-standard chessboard.
    /// </summary>
    [Fact]
    public void Test_Rook_Rectangular_Board()
    {
        var gameDriver = new GameDriver(new Chessboard(4,10));
        
        Assert.True(gameDriver.Board.Insert("r", "b2"));

        Assert.Equal(Constants.BlackRookIdentifier,         gameDriver.Board.GetPieceAsString("b2"));
        Assert.True(gameDriver.Move("b2i2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("b2"));
        Assert.Equal(Constants.BlackRookIdentifier,         gameDriver.Board.GetPieceAsString("i2"));

        Assert.True(gameDriver.Move("i2i4"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("i2"));
        Assert.Equal(Constants.BlackRookIdentifier,         gameDriver.Board.GetPieceAsString("i4"));

    }

    /// <summary>
    /// Tests that invalid moves are not processed.
    /// </summary>
    [Fact]
    public void Test_Invalid_Move()
    {
        var gameDriver = new GameDriver(Chessboard.StandardChessboard());

        Assert.Equal(Constants.WhiteRookIdentifier,         gameDriver.Board.GetPieceAsString("h1"));
        Assert.Equal(Constants.WhitePawnIdentifier,         gameDriver.Board.GetPieceAsString("h2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("h4"));
        Assert.False(gameDriver.Move("h1h4"));
        Assert.Equal(Constants.WhiteRookIdentifier,         gameDriver.Board.GetPieceAsString("h1"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("h4"));
        
        Assert.Equal(Constants.BlackKnightIdentifier,       gameDriver.Board.GetPieceAsString("b8"));
        Assert.Equal(Constants.BlackPawnIdentifier,         gameDriver.Board.GetPieceAsString("d7"));
        Assert.False(gameDriver.Move("b8d7"));
        Assert.Equal(Constants.BlackKnightIdentifier,       gameDriver.Board.GetPieceAsString("b8"));
        Assert.Equal(Constants.BlackPawnIdentifier,         gameDriver.Board.GetPieceAsString("d7"));

        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("e4"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("e5"));
        Assert.False(gameDriver.Move("e4e5"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("e4"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("e5"));

    }

    /// <summary>
    /// Test that a serier of moves can be processed correctly.
    /// </summary>
    [Fact]
    public void Test_Move_Serie()
    {
        var gameDriver = new GameDriver(Chessboard.StandardChessboard());

        Assert.Equal(Constants.WhitePawnIdentifier,         gameDriver.Board.GetPieceAsString("a2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("a3"));
        Assert.True(gameDriver.Move("a2a3"));
        Assert.True(gameDriver.Move("a3a4"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("a2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("a3"));
        Assert.Equal(Constants.WhitePawnIdentifier,         gameDriver.Board.GetPieceAsString("a4"));
    }

    /// <summary>
    /// Tests that a knight can jump over pieces and move correctly.
    /// </summary>
    [Fact]
    public void Test_Move_Knight()
    {
        var gameDriver = new GameDriver(Chessboard.StandardChessboard());

        Assert.Equal(Constants.WhiteKnightIdentifier,       gameDriver.Board.GetPieceAsString("g1"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("h3"));
        Assert.True(gameDriver.Move("g1h3"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("g1"));
        Assert.Equal(Constants.WhiteKnightIdentifier,       gameDriver.Board.GetPieceAsString("h3"));
        Assert.True(gameDriver.Move("h3f4"));
    }
      
    /// <summary>
    /// Test that pices can be captured correctly.
    /// </summary>
    [Fact]
     public void Test_Take()
     {
        var gameDriver = new GameDriver(Chessboard.StandardChessboard());

        Assert.Equal(Constants.WhiteKnightIdentifier,       gameDriver.Board.GetPieceAsString("g1"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("h3"));
        Assert.True(gameDriver.Move("g1h3"));
        Assert.True(gameDriver.Move("h3g5"));
        Assert.True(gameDriver.Move("g5h7"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("g5"));
        Assert.Equal(Constants.WhiteKnightIdentifier,       gameDriver.Board.GetPieceAsString("h7"));
     }
     
     /// <summary>
     /// Test that the bishop can move correctly diagonally.
     /// </summary>
     [Fact]
     public void Test_Bishop()
     {
        var gameDriver = new GameDriver(Chessboard.StandardChessboard());
        Assert.False(gameDriver.Move("f1c4"));
        Assert.True(gameDriver.Move("e2e3"));
        Assert.True(gameDriver.Move("f1c4"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("f1"));
        Assert.Equal(Constants.WhiteBishopIdentifier,       gameDriver.Board.GetPieceAsString("c4"));
     }

    
     [Fact]
     public void Test_Move_King()
     {
        var gameDriver = new GameDriver(Chessboard.StandardChessboard());

        Assert.Equal(Constants.WhiteKingIdentifier,  gameDriver.Board.GetPieceAsString("e1"));
        Assert.True(gameDriver.Move("e2e3"));
        Assert.True(gameDriver.Move("e1e2"));
        Assert.False(gameDriver.Move("e2e3"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  gameDriver.Board.GetPieceAsString("e1"));
        Assert.Equal(Constants.WhiteKingIdentifier,  gameDriver.Board.GetPieceAsString("e2"));
        Assert.Equal(Constants.WhitePawnIdentifier,  gameDriver.Board.GetPieceAsString("e3"));
        Assert.True(gameDriver.Move("e2f3"));

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

    /// <summary>
    /// Test that the queen can move both straight and diagnoally.
    /// </summary>
    [Fact]
    public void Test_Queen()
    {
        var gameDriver = new GameDriver(new Chessboard(6,5));
        
        Assert.True(gameDriver.Board.Insert("Q", "c4"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier, gameDriver.Board.GetPieceAsString("a6"));
        Assert.True(gameDriver.Move("c4a6"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier, gameDriver.Board.GetPieceAsString("c4"));
        Assert.Equal(Constants.WhiteQueenIdentifier, gameDriver.Board.GetPieceAsString("a6"));

        Assert.True(gameDriver.Move("a6a1"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier, gameDriver.Board.GetPieceAsString("a6"));
        Assert.Equal(Constants.WhiteQueenIdentifier, gameDriver.Board.GetPieceAsString("a1"));

    }

    /// <summary>
    /// Test that a piece can repeat its movement pattern.
    /// </summary>
    /*[Fact]
    public void Test_Repeat()
    {
        var gameDriver = new GameDriver(new Chessboard(8));

        gameDriver.Board.Insert("r", "a1");
        Assert.True(gameDriver.Move("a1b2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier, gameDriver.Board.GetPieceAsString("a1"));
        Assert.Equal(Constants.BlackRookIdentifier,  gameDriver.Board.GetPieceAsString("b2"));
    }*/

}
