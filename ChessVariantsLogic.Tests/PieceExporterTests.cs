using System.Collections.Generic;
using ChessVariantsLogic.Export;
using Xunit;

namespace ChessVariantsLogic.Tests;

/// <summary>
/// This class contains unit tests for testing that Piece can be serialized and deserialized correctly.
/// </summary>
public class PieceExporterTests
{
    
    [Fact]
    public void ExportRook_ShouldHave4Patterns()
    {
        var pieceState = PieceExporter.ExportPieceState(Piece.Rook(PieceClassifier.BLACK));
        Assert.Equal(4, pieceState.Movement.Count);
    }

    [Fact]
    public void ExportToJsonAndParseToPiece_ShouldRestorePiece()
    {
        var jsonPiece = Piece.Queen(PieceClassifier.BLACK).ExportAsJson();
        var piece = PieceBuilder.ParseJson(jsonPiece);
        Assert.NotNull(piece);
    }

    [Fact]
    public void SerializeAndDeserializeBishop_MovementShouldRemainSame()
    {
        Piece expected = Piece.Bishop(PieceClassifier.BLACK);
        var jsonPiece = expected.ExportAsJson();
        var actual = PieceBuilder.ParseJson(jsonPiece);
        Assert.Equal(expected.GetAllMovementPatterns(), actual.GetAllMovementPatterns());
    }

    [Fact]
    public void SerializeAndDeserializeRook_CapturesShouldRemainSame()
    {
        Piece expected = Piece.Rook(PieceClassifier.BLACK);
        var jsonPiece = expected.ExportAsJson();
        var actual = PieceBuilder.ParseJson(jsonPiece);
        
        Assert.Equal(expected.GetAllCapturePatterns(), actual.GetAllCapturePatterns());

    }

    [Fact]
    public void ExportCustomPieceWithBothJumpAndRegularMovement_MovementShouldRemainSame()
    {
        var patterns = new List<IPattern> {
            new RegularPattern(Constants.North, 1, 8),
            new JumpPattern(1, 2),
        };
        var mp = new MovementPattern(patterns);

        var expected = new Piece(mp, mp, false, PieceClassifier.WHITE, 1, "ca", true);
        string jsonPiece = expected.ExportAsJson();
        var actual = PieceBuilder.ParseJson(jsonPiece);

        Assert.Equal(expected.GetAllMovementPatterns(), actual.GetAllMovementPatterns());
    }

    [Fact]
    public void SeparateMovementAndCapture_MovementAndCaptureShouldRemainSeparate()
    {
        var movements = new List<IPattern> {
            new RegularPattern(Constants.North, 1, 8),
            new JumpPattern(1, 2),
        };
        var mp = new MovementPattern(movements);

        var captures = new List<IPattern> {
            new RegularPattern(Constants.West, 1, 8),
            new JumpPattern(3, 1),
        };
        var cp = new MovementPattern(captures);

        var expected = new Piece(mp, cp, false, PieceClassifier.WHITE, 1, "ca", true);
        string jsonPiece = expected.ExportAsJson();
        var actual = PieceBuilder.ParseJson(jsonPiece);

        Assert.Equal(expected.GetAllMovementPatterns(), actual.GetAllMovementPatterns());
        Assert.Equal(expected.GetAllCapturePatterns(), actual.GetAllCapturePatterns());
        Assert.NotEqual(expected.GetAllMovementPatterns(), actual.GetAllCapturePatterns());
    }


}
