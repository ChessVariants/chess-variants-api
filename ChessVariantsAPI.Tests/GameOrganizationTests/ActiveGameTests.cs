using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChessVariantsAPI.Tests.GameOrganizationTests;
public class ActiveGameTests : IDisposable
{
    ActiveGame activeGame;

    public ActiveGameTests()
    {
        activeGame = new ActiveGame(GameFactory.StandardChess(), "admin");
    }

    public void Dispose()
    {
        activeGame = new ActiveGame(GameFactory.StandardChess(), "admin");
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Constructor_ShouldAddCreatorAsWhite()
    {
        Assert.Equal(Player.White, activeGame.GetPlayer("admin"));
    }

    [Fact]
    public void AddPlayer_ShouldAddABlackPlayer()
    {
        activeGame.AddPlayer("newPlayerId");
        var newPlayer = activeGame.GetPlayer("newPlayerId");
        Assert.Equal(Player.Black, newPlayer);
    }

    [Fact]
    public void AddPlayer_ShouldThrowExceptionIfThirdPlayer()
    {
        activeGame.AddPlayer("playerTwo");
        Assert.Throws<GameFullException>(() => activeGame.AddPlayer("playerThree"));
    }

    [Fact]
    public void LeaveGame_ShouldAssignNewAdminIfAdminLeaves()
    {
        activeGame.AddPlayer("playerTwo");
        Assert.Equal("admin", activeGame.Admin);
        activeGame.RemovePlayer("admin");
        Assert.Equal("playerTwo", activeGame.Admin);
    }

    [Fact]
    public void LeaveGame_ShouldThrowExceptionIfGameBecomesEmpty()
    {
        Assert.Throws<GameEmptyException>(() => activeGame.RemovePlayer("admin"));
    }

    [Fact]
    public void SwapColors_ShouldSwapColorsTwoPlayers()
    {
        activeGame.AddPlayer("playerTwo");
        Assert.Equal(Player.White, activeGame.GetPlayer("admin"));
        Assert.Equal(Player.Black, activeGame.GetPlayer("playerTwo"));
        activeGame.SwapColors("admin");
        Assert.Equal(Player.Black, activeGame.GetPlayer("admin"));
        Assert.Equal(Player.White, activeGame.GetPlayer("playerTwo"));
    }

    [Fact]
    public void SwapColors_ShouldSwapColorsOnePlayer()
    {
        Assert.Equal(Player.White, activeGame.GetPlayer("admin"));
        activeGame.SwapColors("admin");
        Assert.Equal(Player.Black, activeGame.GetPlayer("admin"));
    }

    [Fact]
    public void SwapColors_ShouldNotSwapColorsIfRequesterNotAdmin()
    {
        activeGame.AddPlayer("playerTwo");
        Assert.Equal(Player.White, activeGame.GetPlayer("admin"));
        Assert.Equal(Player.Black, activeGame.GetPlayer("playerTwo"));
        activeGame.SwapColors("playerTwo");
        Assert.Equal(Player.White, activeGame.GetPlayer("admin"));
        Assert.Equal(Player.Black, activeGame.GetPlayer("playerTwo"));
    }


}
