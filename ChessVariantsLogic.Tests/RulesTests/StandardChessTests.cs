using Xunit;
using System;

namespace ChessVariantsLogic.Tests;

public class StandardChessTest : IDisposable {

    Game game;
    Player w = Player.White;
    Player b = Player.Black;

    public StandardChessTest()
    {
        game = GameFactory.StandardChess();
    }

    public void Dispose()
    {
        game = GameFactory.StandardChess();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void CheckmateWithRegularMovementPattern_ShouldReturnWhiteWon()
    {
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
    public void CheckmateWithRegularMovementPattern_ShouldReturnBlackWon()
    {
        game.MakeMove("b1a3", w);
        game.MakeMove("e7e5", b);
        game.MakeMove("a3b1", w);
        game.MakeMove("f8c5", b);
        game.MakeMove("b1a3", w);
        game.MakeMove("d8f6", b);
        game.MakeMove("a3b1", w);
        var actual = game.MakeMove("f6f2", b);

        Assert.Contains(GameEvent.BlackWon, actual);

    }

    [Fact]
    public void CheckmateWithJumpMovementPattern_ShouldReturnWhiteWon()
    {
        game.MakeMove("e2e4", w);
        game.MakeMove("b8a6", b);
        game.MakeMove("g1f3", w);
        game.MakeMove("a6b8", b);
        game.MakeMove("f3e5", w);
        game.MakeMove("b8a6", b);
        game.MakeMove("d1f3", w);
        game.MakeMove("a6b8", b);

        var actual = game.MakeMove("f3f7", w);

        Assert.Contains(GameEvent.WhiteWon, actual);

    }

    [Fact]
    public void CheckmateWithJumpMovementPattern_ShouldReturnBlackWon()
    {
        game.MakeMove("b1a3", w);
        game.MakeMove("e7e5", b);
        game.MakeMove("a3b1", w);
        game.MakeMove("g8f6", b);
        game.MakeMove("b1a3", w);
        game.MakeMove("f6e4", b);
        game.MakeMove("a3b1", w);
        game.MakeMove("d8f6", b);
        game.MakeMove("b1a3", w);
        
        var actual = game.MakeMove("f6f2", b);

        Assert.Contains(GameEvent.BlackWon, actual);

    }

    [Fact]
    public void PositionThatIsStaleMate_ShouldReturnTie()
    {
        // This was the fastest way I found to achieve stalemate.. It's surprisingly quick, but I suppose the engine would not be too impressed :D
        game.MakeMove("e2e3", w);
        game.MakeMove("a7a5", b);
        game.MakeMove("d1h5", w);
        game.MakeMove("a8a6", b);
        game.MakeMove("h5a5", w);
        game.MakeMove("h7h5", b);
        game.MakeMove("h2h4", w);
        game.MakeMove("a6h6", b);
        game.MakeMove("a5c7", w);
        game.MakeMove("f7f6", b);
        game.MakeMove("c7d7", w);
        game.MakeMove("e8f7", b);
        game.MakeMove("d7b7", w);
        game.MakeMove("d8d3", b);
        game.MakeMove("b7b8", w);
        game.MakeMove("d3h7", b);
        game.MakeMove("b8c8", w);
        game.MakeMove("f7g6", b);
        
        var actual = game.MakeMove("c8e6", w);

        Assert.Contains(GameEvent.Tie, actual);

    }



}