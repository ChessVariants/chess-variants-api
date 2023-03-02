using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic;
using System;
using Xunit;

namespace ChessVariantsAPI.Tests;

public class GameOrganizerTests : IDisposable
{
    private GameOrganizer gameOrganizer;

    public GameOrganizerTests()
    {
        gameOrganizer = new GameOrganizer();
        gameOrganizer.CreateGame("0", "id");
    }

    public void Dispose()
    {
        gameOrganizer = new GameOrganizer();
        gameOrganizer.CreateGame("0", "id");
        GC.SuppressFinalize(this);
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
}
