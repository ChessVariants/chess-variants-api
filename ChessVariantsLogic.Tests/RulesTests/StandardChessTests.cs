using Xunit;
using System;

namespace ChessVariantsLogic.Tests;

public class StandardChessTest : IDisposable {

    public StandardChessTest()
    {

    }

    public void Dispose()
    {
        //board = new MoveWorker(new Chessboard(8));
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void CheckmateWithRegularMovementPattern_ShouldReturnWhiteWon()
    {
        var game = GameFactory.StandardChess();
        var w = Player.White;
        var b = Player.Black;

        game.MakeMove("e2e4", w);
        game.MakeMove("b8a6", b);
        game.MakeMove("f1c4", w);
        game.MakeMove("a6b8", b);
        game.MakeMove("d1f3", w);
        game.MakeMove("b8a6", b);
        var actual = game.MakeMove("f3f7", w);

        Assert.Contains(GameEvent.WhiteWon, actual);

    }

    [Fact]
    public void CheckmateWithJumpMovementPattern_ShouldReturnWhiteWon()
    {
        var game = GameFactory.StandardChess();
        var w = Player.White;
        var b = Player.Black;

        game.MakeMove("e2e4", w);
        game.MakeMove("b8a6", b);
        game.MakeMove("g1f3", w);
        game.MakeMove("a6b8", b);
        game.MakeMove("f3e5", w);
        game.MakeMove("b8a6", b);
        game.MakeMove("d1f3", w);
        game.MakeMove("a6b8", b);
        var actual = game.MakeMove("f3f7", w);
        game.MakeMove("e8f7", b);
        var moves = game.MoveWorker.GetAllCapturePatternMoves(w);

        Assert.Contains(GameEvent.WhiteWon, actual);

    }



}