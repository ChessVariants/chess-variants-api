using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace ChessVariantsAPI.Tests;

public class GameOrganizerTests
{
    private class MockLogger : ILogger<GameOrganizer>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            return;
        }
    }

    private GameOrganizer gameOrganizer;

    public GameOrganizerTests()
    {
        gameOrganizer = new GameOrganizer(new MockLogger());
        gameOrganizer.CreateGame("0", "id");
    }

    [Fact]
    public void GetGame_ShouldReturnGameIfItExists()
    {
        var game = gameOrganizer.GetGame("0");
        Assert.NotNull(game); // GetGame cannot return null value, this is just to make sure no exceptions are thrown
    }

    [Fact]
    public void GetGame_ShouldThrowExceptionIfNoGameExists()
    {
        Assert.Throws<GameNotFoundException>(() => gameOrganizer.GetGame("non_existing_game_id"));
    }

    [Fact]
    public void CreateNewGame_ShouldAddGame()
    {
        gameOrganizer.CreateGame("1", "id");
        var game = gameOrganizer.GetGame("1");
        Assert.NotNull(game);
    }

    [Fact]
    public void CreateGame_ShouldThrowExceptionIfGameAlreadyExists()
    {
        Assert.Throws<OrganizerException>(() => gameOrganizer.CreateGame("0", "id"));
    }

    [Fact]
    public void JoinGame_ShouldWork()
    {
        gameOrganizer.JoinGame("0", "second_id");
        Assert.True(true); // Might look weird but this test is simply to make sure no exceptions are thrown.
    }

    [Fact]
    public void LeaveGame_ShouldReturnTrueIfPlayerInGameAndGameStillExists()
    {
        gameOrganizer.JoinGame("0", "second_id");
        var result = gameOrganizer.LeaveGame("0", "second_id");
        Assert.True(result);
    }

    [Fact]
    public void LeaveGame_ShouldReturnFalseIfPlayerNotInGame()
    {
        var result = gameOrganizer.LeaveGame("0", "second_id");
        Assert.False(result);
    }

    [Fact]
    public void LeaveGame_ShouldReturnFalseIfGameDoesntExist()
    {
        var result = gameOrganizer.LeaveGame("1", "id");
        Assert.False(result);
    }

    [Fact]
    public void GetPlayer_ShouldReturnPlayerIfExists()
    {
        var player = gameOrganizer.GetPlayer("0", "id");
        Assert.IsType<Player>(player); // just to make sure no exceptions
    }

    [Fact]
    public void GetPlayer_ShouldThrowExceptionIfPlayerDoesntExist()
    {
        Assert.Throws<PlayerNotFoundException>(() => gameOrganizer.GetPlayer("0", "non_existing_id"));
    }

    [Fact]
    public void CreateGame_ShouldBeStandardVariant()
    {
        Assert.Equal(gameOrganizer.GetActiveGameVariant("0"), GameFactory.StandardIdentifier);
    }

    [Fact]
    public void CreateGame_ShouldBeCaptureTheKing()
    {
        gameOrganizer.CreateGame("1", "id", GameFactory.CaptureTheKingIdentifier);
        Assert.Equal(gameOrganizer.GetActiveGameVariant("1"), GameFactory.CaptureTheKingIdentifier);
    }
}
