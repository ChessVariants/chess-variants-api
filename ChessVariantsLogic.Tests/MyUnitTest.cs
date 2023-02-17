using System;
using Xunit;

namespace ChessVariantsLogic.Tests;

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


}
