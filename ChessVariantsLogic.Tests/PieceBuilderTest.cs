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
        this.builder.AddMovementPattern(Constants.North, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.East,  1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.South, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.West,  1, Constants.MaxBoardHeight);
        
        this.builder.SetSameMovementAndCapturePattern(true);
        this.builder.BelongsToPlayer("white");

        Piece customRook = this.builder.Build();

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
        this.builder.BelongsToPlayer("white");
        Assert.Throws<ArgumentException>(() => this.builder.Build());
    }

    [Fact]
    public void BuildPieceWithSeperateCaptureAndMovementPatterns()
    {      
        this.builder.AddMovementPattern(Constants.North, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.East,  1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.South, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.West,  1, Constants.MaxBoardHeight);

        this.builder.AddJumpCapturePattern( 2,  2);
        this.builder.AddJumpCapturePattern( 2, -2);
        this.builder.AddJumpCapturePattern(-2,  2);
        this.builder.AddJumpCapturePattern(-2, -2);

        this.builder.SetSameMovementAndCapturePattern(false);
        this.builder.BelongsToPlayer("black");

        Piece piece = this.builder.Build();

        this.moveWorker.InsertOnBoard(piece, "e4");
        this.moveWorker.InsertOnBoard(Piece.WhitePawn(), "h6");

        this.moveWorker.Move("e4e8");
        this.moveWorker.Move("e8f8");
        this.moveWorker.Move("f8g7");
        this.moveWorker.Move("f8h6");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier, this.moveWorker.Board.GetPieceIdentifier("f8"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier, this.moveWorker.Board.GetPieceIdentifier("g7"));
        Assert.Equal("ca", this.moveWorker.Board.GetPieceIdentifier("h6"));
        
    }

    [Fact]
    public void CurrentlyValidMovesWorksCorrectly()
    {
        this.builder.AddMovementPattern(Constants.North, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.East,  1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.South, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.West,  1, Constants.MaxBoardHeight);
        this.builder.AddJumpMovementPattern(1, -2);
        this.builder.AddJumpMovementPattern(2, -1);
        
        var moves = this.builder.GetAllCurrentlyValidMovesFromSquare("e4");
        
        Assert.Equal(16, moves.Count);
    }

}