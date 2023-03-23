namespace ChessVariantsLogic.Tests;

using System;
using Xunit;

/// <summary>
/// This class contains unit tests for the class PieceBuilder.
/// </summary>
public class PieceBuilderTest : IDisposable
{

    private MoveWorker moveWorker;
    private PieceBuilder builder;

    public PieceBuilderTest()
    {
        this.moveWorker = new MoveWorker(new Chessboard(8));
        this.builder = new PieceBuilder();
    }

    public void Dispose()
    {
        this.moveWorker = new MoveWorker(new Chessboard(8));
        this.builder.Reset();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void PieceBuilderCanBuildRook()
    {
        this.builder.Reset();

        builder.AddMovementPattern(Constants.North, 1, Constants.MaxBoardHeight);
        builder.AddMovementPattern(Constants.East,  1, Constants.MaxBoardHeight);
        builder.AddMovementPattern(Constants.South, 1, Constants.MaxBoardHeight);
        builder.AddMovementPattern(Constants.West,  1, Constants.MaxBoardHeight);
        
        builder.SetSameMovementAndCapturePattern(true);
        builder.BelongsToPlayer("white");

        Piece customRook = builder.Build();

        this.moveWorker.InsertOnBoard(customRook, "e4");
        this.moveWorker.Move("e4e8");
        this.moveWorker.Move("e8f8");
        this.moveWorker.Move("f8g7");
        
        Assert.Equal("CA", this.moveWorker.Board.GetPieceIdentifier("f8"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier, this.moveWorker.Board.GetPieceIdentifier("g7"));
    }

    [Fact]
    public void PieceBuilderThrowsExceptionWhenMovementPatternIsMissing()
    {
        var builder = new PieceBuilder();
        builder.BelongsToPlayer("white");

        Assert.Throws<ArgumentException>(() => builder.Build());
    }

    [Fact]
    public void PieceBuilderThrowsExceptionWhenPieceClassifierIsMissing()
    {
        var builder = new PieceBuilder();
        
        builder.AddMovementPattern(Constants.North, 1, Constants.MaxBoardHeight);
        builder.AddMovementPattern(Constants.East,  1, Constants.MaxBoardHeight);
        builder.AddMovementPattern(Constants.South, 1, Constants.MaxBoardHeight);
        builder.AddMovementPattern(Constants.West,  1, Constants.MaxBoardHeight);

        Assert.Throws<ArgumentException>(() => builder.Build());
    }
}