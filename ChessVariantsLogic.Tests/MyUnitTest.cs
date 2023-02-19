using System;
using Xunit;

namespace ChessVariantsLogic.Tests;

// These tests are obviously not done, they have been used for manual debugging so don't worry :))
public class MyUnitTest
{

    [Fact]
    public void Print_Chessboard()
    {
        var board = Chessboard.StandardChessboard();
        Console.WriteLine(board.ToString());
        board.MakeMove("e2e4");
        Console.WriteLine(board.ToString());

        Assert.True(true);
    }

    [Fact]
    public void Print_FEN()
    {
        var board = Chessboard.StandardChessboard();
        Console.WriteLine(board.ReadBoardAsFEN());
        board.MakeMove("h2h4");
        Console.WriteLine(board.ReadBoardAsFEN());

        Assert.True(true);
    }

    [Fact]
    public void Test_sizes()
    {
        var board1 = new Chessboard(6);
        Console.WriteLine(board1.ToString());

        var board2 = new Chessboard(6,15);
        Console.WriteLine(board2.ToString());
        
        var board3 = new Chessboard(10,1);
        Console.WriteLine(board3.ToString());

        Assert.True(true);
    }


}
