using System;
namespace ChessVariantsLogic.Tests;

using System.Collections.Generic;
using System.Linq;
using Xunit;


public class GameTests
{
    Game game;

    public GameTests()
    {
        game = GameFactory.StandardChess();
    }

    [Fact]
    public void GetPromotablePieces_ShouldOnlyReturnPiecesForWhite()
    {
        var promotablePieces = game.GetPromotablePieces();
        Assert.All(
            promotablePieces,
            piece => Assert.True(piece.PieceClassifier == PieceClassifier.WHITE)
            );
    }

    [Fact]
    public void GetPromotablePieces_ShouldContainAllButKingAndPawn()
    {
        var promotablePieces = game.GetPromotablePieces();
        Assert.Equal(1, promotablePieces.Count(p => p.PieceIdentifier == Constants.WhiteBishopIdentifier));
        Assert.Equal(1, promotablePieces.Count(p => p.PieceIdentifier == Constants.WhiteKnightIdentifier));
        Assert.Equal(1, promotablePieces.Count(p => p.PieceIdentifier == Constants.WhiteQueenIdentifier));
        Assert.Equal(1, promotablePieces.Count(p => p.PieceIdentifier == Constants.WhiteRookIdentifier));
        Assert.Equal(0, promotablePieces.Count(p => p.PieceIdentifier == Constants.WhiteKingIdentifier));
        Assert.Equal(0, promotablePieces.Count(p => p.PieceIdentifier == Constants.WhitePawnIdentifier));
        Assert.Equal(4, promotablePieces.Count());
    }
}

