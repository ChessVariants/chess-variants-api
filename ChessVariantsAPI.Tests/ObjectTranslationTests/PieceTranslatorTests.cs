using ChessVariantsAPI.ObjectTranslations;
using Xunit;

namespace ChessVariantsAPI.Tests.ObjectTranslationTests;
public class PieceTranslatorTests
{
    [Fact]
    public void CreatePieceModel_ShouldNotThrowException()
    {
        var p = ChessVariantsLogic.Piece.BlackPawn();
        var exception = Record.Exception(() => PieceTranslator.CreatePieceModel(p, "test-piece", "me", "pawn.png"));
        Assert.Null(exception);
    }
}
