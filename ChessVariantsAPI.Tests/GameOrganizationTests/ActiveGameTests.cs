using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChessVariantsAPI.Tests.GameOrganizationTests;
public class ActiveGameTests
{
    ActiveGame activeGame;

    public ActiveGameTests()
    {
        activeGame = new ActiveGame(GameFactory.StandardChess(), "admin", GameFactory.StandardIdentifier);
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
    public void GetPlayers_ShouldHave2Players()
    {
        activeGame.AddPlayer("newPlayerId");
        var players = activeGame.GetPlayers();
        Assert.Equal(2, players.Count);
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

    [Fact]
    public void Constructor_ShouldBeStandardVariant()
    {
        activeGame = new ActiveGame(GameFactory.FromIdentifier(GameFactory.StandardIdentifier), "admin", GameFactory.StandardIdentifier);
        Assert.Equal(GameFactory.StandardIdentifier, activeGame.Variant);
    }

    [Fact]
    public void Constructor_ShouldBeStandardAntiChess()
    {
        activeGame = new ActiveGame(GameFactory.FromIdentifier(GameFactory.AntiChessIdentifier), "admin", GameFactory.AntiChessIdentifier);
        Assert.Equal(GameFactory.AntiChessIdentifier, activeGame.Variant);
    }


}
