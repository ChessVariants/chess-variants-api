using ChessVariantsAPI.GameOrganization;
using System;
using Xunit;

namespace ChessVariantsAPI.Tests;

public class GameOrganizerTests : IDisposable
{
    private GameOrganizer _gameOrganizer;

    public GameOrganizerTests()
    {
        _gameOrganizer = new GameOrganizer();
    }

    public void Dispose()
    {
        _gameOrganizer = new GameOrganizer();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GetGameShouldThrowExceptionIfNoGameExists()
    {
        Assert.Throws<GameNotFoundException>(() => _gameOrganizer.GetGame("0"));
    }

    [Fact]
    public void CreateNewGameShouldAddGame()
    {
        _gameOrganizer.JoinGame("0", "id", "white");
        var game = _gameOrganizer.GetGame("0");
        Assert.True(game != null);
    }
}
