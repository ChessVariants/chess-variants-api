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
        var board = new Chessboard(6);
        Assert.Equal("6/6/6/6/6/6", board.ReadBoardAsFEN());

        board = new Chessboard(12, 3);
        Assert.Equal("3/3/3/3/3/3/3/3/3/3/3/3", board.ReadBoardAsFEN());

        board = Chessboard.StandardChessboard();
        Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", board.ReadBoardAsFEN());

        board.MakeMove("a2a4");
        Assert.Equal("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR", board.ReadBoardAsFEN());

    }

    /// <summary>
    /// Tests that the standard chessboard is set up correctly.
    /// </summary>
    [Fact]
    public void Test_Standard_Chessboard()
    {
        var board = Chessboard.StandardChessboard();
        Assert.Equal(Constants.WhiteKingIdentifier,         board.GetPiece("e1"));
        Assert.Equal(Constants.BlackQueenIdentifier,        board.GetPiece("d8"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  board.GetPiece("e4"));
        Assert.Equal(Constants.BlackRookIdentifier,         board.GetPiece("a8"));
    }

    /// <summary>
    /// Tests that moving a piece updates the board correctly.
    /// </summary>
    [Fact]
    public void Test_Move_Piece()
    {
        var board = Chessboard.StandardChessboard();
        Assert.Equal(Constants.WhitePawnIdentifier,         board.GetPiece("g2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  board.GetPiece("g4"));
        board.MakeMove("g2g4");
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  board.GetPiece("g2"));
        Assert.Equal(Constants.WhitePawnIdentifier,         board.GetPiece("g4"));
    }


}
