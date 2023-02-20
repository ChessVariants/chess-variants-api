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
        var board1 = new Chessboard(6);
        string fen1 = board1.ReadBoardAsFEN();
        string[] splitFen1 = fen1.Split('/');
        Assert.Equal(6, splitFen1.Length);

        var board2 = new Chessboard(12, 3);
        string fen2 = board2.ReadBoardAsFEN();
        string[] splitFen2 = fen2.Split('/');
        Assert.Equal(12, splitFen2.Length);

        var stdBoard = Chessboard.StandardChessboard();
        string fen3 = stdBoard.ReadBoardAsFEN();
        Assert.Equal("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", fen3);

        stdBoard.MakeMove("a2a4");
        string fen4 = stdBoard.ReadBoardAsFEN();
        Assert.Equal("rnbqkbnr/pppppppp/8/8/P7/8/1PPPPPPP/RNBQKBNR", fen4);

    }

    /// <summary>
    /// Tests that the standard chessboard is set up correctly.
    /// </summary>
    [Fact]
    public void Test_Standard_Chessboard()
    {
        var board = Chessboard.StandardChessboard();
        Assert.Equal(Constants.WhiteKingIdentifier,         board.GetPiece(board.CoorToIndex["e1"]));
        Assert.Equal(Constants.BlackQueenIdentifier,        board.GetPiece(board.CoorToIndex["d8"]));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  board.GetPiece(board.CoorToIndex["e4"]));
        Assert.Equal(Constants.BlackRookIdentifier,         board.GetPiece(board.CoorToIndex["a8"]));
    }

    /// <summary>
    /// Tests that moving a piece updates the board correctly.
    /// </summary>
    [Fact]
    public void Test_Move_Piece()
    {
        var board = Chessboard.StandardChessboard();
        Assert.Equal(Constants.WhitePawnIdentifier,         board.GetPiece(board.CoorToIndex["g2"]));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  board.GetPiece(board.CoorToIndex["g4"]));
        board.MakeMove("g2g4");
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  board.GetPiece(board.CoorToIndex["g2"]));
        Assert.Equal(Constants.WhitePawnIdentifier,         board.GetPiece(board.CoorToIndex["g4"]));
    }


}
