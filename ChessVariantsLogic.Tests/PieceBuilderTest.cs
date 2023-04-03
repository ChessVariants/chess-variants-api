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
    public void RegularPatternsCanBeRemovedFromMovement()
    {      
        this.builder.AddMovementPattern(Constants.North, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.East,  1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.South, 1, Constants.MaxBoardHeight);
        this.builder.AddMovementPattern(Constants.West,  1, Constants.MaxBoardHeight);

        var actual = this.builder.GetAllCurrentlyValidMovesFromSquare("a1").Count;

        this.builder.RemoveMovementPattern(Constants.North, 1, Constants.MaxBoardHeight);
        this.builder.RemoveMovementPattern(Constants.East,  1, Constants.MaxBoardHeight);
        this.builder.RemoveMovementPattern(Constants.South, 1, Constants.MaxBoardHeight);
        this.builder.RemoveMovementPattern(Constants.West,  1, Constants.MaxBoardHeight);

        Assert.Equal(14, actual);
        Assert.Equal(0, this.builder.GetAllCurrentlyValidMovesFromSquare("a1").Count);
        
    }

    [Fact]
    public void JumpPatternsCanBeRemovedFromMovement()
    {      
        this.builder.AddJumpMovementPattern(1,  2);
        this.builder.AddJumpMovementPattern(1, -2);
        this.builder.AddJumpMovementPattern(-1, 2);
        this.builder.AddJumpMovementPattern(-1, -2);

        var actual = this.builder.GetAllCurrentlyValidMovesFromSquare("e4").Count;

        this.builder.RemoveJumpMovementPattern(1,  2);
        this.builder.RemoveJumpMovementPattern(1, -2);
        this.builder.RemoveJumpMovementPattern(-1, 2);
        this.builder.RemoveJumpMovementPattern(-1, -2);

        Assert.Equal(4, actual);
        Assert.Equal(0, this.builder.GetAllCurrentlyValidMovesFromSquare("a1").Count);
    }

    [Fact]
    public void RegularPatternsCanBeRemovedFromCaptures()
    { 
        this.builder.SetSameMovementAndCapturePattern(false);

        this.builder.AddCapturePattern(Constants.North, 1, Constants.MaxBoardHeight);
        this.builder.AddCapturePattern(Constants.East,  1, Constants.MaxBoardHeight);
        this.builder.AddCapturePattern(Constants.South, 1, Constants.MaxBoardHeight);
        this.builder.AddCapturePattern(Constants.West,  1, Constants.MaxBoardHeight);

        var actual = this.builder.GetAllCurrentlyValidCaptureMovesFromSquare("a1").Count;

        this.builder.RemoveCapturePattern(Constants.North, 1, Constants.MaxBoardHeight);
        this.builder.RemoveCapturePattern(Constants.East,  1, Constants.MaxBoardHeight);
        this.builder.RemoveCapturePattern(Constants.South, 1, Constants.MaxBoardHeight);
        this.builder.RemoveCapturePattern(Constants.West,  1, Constants.MaxBoardHeight);

        Assert.Equal(14, actual);
        Assert.Equal(0, this.builder.GetAllCurrentlyValidCaptureMovesFromSquare("a1").Count);
        
    }

    [Fact]
    public void JumpPatternsCanBeRemovedFromCaptures()
    {
        this.builder.SetSameMovementAndCapturePattern(false);

        this.builder.AddJumpCapturePattern(1,  2);
        this.builder.AddJumpCapturePattern(1, -2);
        this.builder.AddJumpCapturePattern(-1, 2);
        this.builder.AddJumpCapturePattern(-1, -2);

        var actual = this.builder.GetAllCurrentlyValidCaptureMovesFromSquare("e4").Count;

        this.builder.RemoveJumpCapturePattern(1,  2);
        this.builder.RemoveJumpCapturePattern(1, -2);
        this.builder.RemoveJumpCapturePattern(-1, 2);
        this.builder.RemoveJumpCapturePattern(-1, -2);

        Assert.Equal(4, actual);
        Assert.Equal(0, this.builder.GetAllCurrentlyValidCaptureMovesFromSquare("a1").Count);
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