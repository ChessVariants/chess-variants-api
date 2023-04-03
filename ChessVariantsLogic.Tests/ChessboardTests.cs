using ChessVariantsLogic.Rules.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ChessVariantsLogic.Tests;

/// <summary>
/// This class contains unit tests on Chessboard and MoveWorker.
/// </summary>
public class ChessboardTests : IDisposable
{
    private MoveWorker moveWorker;
    private Game game;
    private const string customPieceNotation = "CA";

    private Perft perft;
   
    

    public ChessboardTests()
    {
        this.moveWorker = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        this.game = GameFactory.StandardChess();
        this.perft = GameFactory.StandardChessPerft();
    }

    public void Dispose()
    {
        this.moveWorker = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Tests that the standard chessboard is set up correctly.
    /// </summary>
    [Fact]
    public void StandardChessboardIsSetUpCorrectly()
    {
        Assert.Equal(Constants.WhiteKingIdentifier,         this.moveWorker.Board.GetPieceIdentifier("e1"));
        Assert.Equal(Constants.BlackQueenIdentifier,        this.moveWorker.Board.GetPieceIdentifier("d8"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("e4"));
        Assert.Equal(Constants.BlackRookIdentifier,         this.moveWorker.Board.GetPieceIdentifier("a8"));
    }

    /// <summary>
    /// Tests that moving a piece updates the board correctly.
    /// </summary>
    [Fact]
    public void PawnMovesCorrectly()
    {
        Assert.Equal(Constants.WhitePawnIdentifier,         this.moveWorker.Board.GetPieceIdentifier("g2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("g3"));
        this.moveWorker.Move("g2g3");
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("g2"));
        Assert.Equal(Constants.WhitePawnIdentifier,         this.moveWorker.Board.GetPieceIdentifier("g3"));
        
        this.moveWorker.Move("h2h9");
        Assert.Equal(Constants.WhitePawnIdentifier,         this.moveWorker.Board.GetPieceIdentifier("h2"));

    }

    /// <summary>
    /// Test that a rook can move correcly on non-standard chessboard.
    /// </summary>
    [Fact]
    public void RookMovesCorrectlyOnRectangularBoard()
    {
        this.moveWorker.Board = new Chessboard(4,10);
        
        this.moveWorker.InsertOnBoard(Piece.Rook(PieceClassifier.BLACK), "b2");
        this.moveWorker.Move("b2i2");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("b2"));
        Assert.Equal(Constants.BlackRookIdentifier,         this.moveWorker.Board.GetPieceIdentifier("i2"));

        this.moveWorker.Move("i2i4");
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("i2"));
        Assert.Equal(Constants.BlackRookIdentifier,         this.moveWorker.Board.GetPieceIdentifier("i4"));

    }

    /// <summary>
    /// Tests that a piece that is not allowed to jump, can not jump over other pieces.
    /// </summary>
    [Fact]
    public void PieceWithoutJumpPattern_CannotJumpOverOtherPieces()
    {
        this.moveWorker.Move("h1h4");
        this.moveWorker.Move("c8e6");

        Assert.Equal(Constants.WhiteRookIdentifier,     this.moveWorker.Board.GetPieceIdentifier("h1"));
        Assert.Equal(Constants.BlackBishopIdentifier,   this.moveWorker.Board.GetPieceIdentifier("c8"));
    }

    /// <summary>
    /// Tests that a piece can not be moved to a square occupied by a piece of the same color.
    /// </summary>
    [Fact]
    public void MovePieceToOccupiedSquare()
    {
        this.moveWorker.Move("h1h2");
        this.moveWorker.Move("b8d7");

        Assert.Equal(Constants.WhiteRookIdentifier,     this.moveWorker.Board.GetPieceIdentifier("h1"));
        Assert.Equal(Constants.BlackKnightIdentifier,   this.moveWorker.Board.GetPieceIdentifier("b8"));
    }

    /// <summary>
    /// Tests that an unnoccupied square can not be moved.
    /// </summary>
    [Fact]
    public void MoveUnnoccupiedSquare()
    {
        this.moveWorker.Move("e4e5");
        Assert.Equal(0, this.moveWorker.Movelog.Count);
    }

    /// <summary>
    /// Test that a serier of moves can be processed correctly.
    /// </summary>
    [Fact]
    public void PerformSeriesOfMoves()
    {
        this.moveWorker.Move("a2a3");
        this.moveWorker.Move("a3a4");
        this.moveWorker.Move("a4a5");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("a2"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("a3"));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("a4"));
        Assert.Equal(Constants.WhitePawnIdentifier,         this.moveWorker.Board.GetPieceIdentifier("a5"));
    }

    /// <summary>
    /// Tests that a knight can jump over pieces and move correctly.
    /// </summary>
    [Fact]
    public void KnightCanMoveProperly()
    {
        this.moveWorker.Move("g1h3");
        this.moveWorker.Move("h3f4");
        Assert.Equal(Constants.WhiteKnightIdentifier, this.moveWorker.Board.GetPieceIdentifier("f4"));
    }
      
    /// <summary>
    /// Test that a knight can capture other pieces correctly.
    /// </summary>
    [Fact]
     public void KnightCanCaptureCorrectly()
     {
        this.moveWorker.Move("g1h3");
        this.moveWorker.Move("h3g5");
        this.moveWorker.Move("g5h7");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("g5"));
        Assert.Equal(Constants.WhiteKnightIdentifier,       this.moveWorker.Board.GetPieceIdentifier("h7"));
     }

    /// <summary>
    /// Tests that a bishop, a piece that can not jump, can capture opponents pieces.
    /// </summary>
     [Fact]
     public void NonJumpablePieceCanCapture()
     {
        this.moveWorker.Move("e2e3");
        this.moveWorker.Move("f1a6");
        this.moveWorker.Move("a6b7");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("a6"));
        Assert.Equal(Constants.WhiteBishopIdentifier,       this.moveWorker.Board.GetPieceIdentifier("b7"));
     }

    /// <summary>
    /// Tests that a pawn can capture diagonally.
    /// </summary>
    [Fact]
     public void PawnCanCaptureDiagonally()
     {
        this.moveWorker.Move("e2e3");
        this.moveWorker.Move("e3e4");
        this.moveWorker.Move("e4e5");
        this.moveWorker.Move("e5e6");
        this.moveWorker.Move("e6d7");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("e6"));
        Assert.Equal(Constants.WhitePawnIdentifier,         this.moveWorker.Board.GetPieceIdentifier("d7"));
     }
     
    /// <summary>
    /// Test that the bishop can move correctly diagonally.
    /// </summary>
    [Fact]
    public void BishopCanMoveDiagonally()
    {
       this.moveWorker.Move("e2e3");
       this.moveWorker.Move("f1c4");
        
       Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("f1"));
       Assert.Equal(Constants.WhiteBishopIdentifier,       this.moveWorker.Board.GetPieceIdentifier("c4"));
    }

    /// <summary>
    /// Test that the king can move correctly.
    /// </summary>
    [Fact]
    public void KingCanMoveCorrectly()
    {
       this.moveWorker.Move("e2e3");
       this.moveWorker.Move("e1e2");
       this.moveWorker.Move("e2f3");

       Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("e1"));
       Assert.Equal(Constants.WhiteKingIdentifier,         this.moveWorker.Board.GetPieceIdentifier("f3"));
    }

    [Theory]
    [InlineData(2, 9)]
    [InlineData(9, 9)]
    [InlineData(10, 7)]
    public void FaultyIndicesByIndex_shouldReturnFalse(int row, int col)
    {
        Assert.False(this.moveWorker.Board.Insert(Constants.BlackBishopIdentifier, row, col));
    }

    [Theory]
    [InlineData("j13")]
    [InlineData("n3")]
    public void FaultyIndicesByString_shouldReturnFalse(string square)
    {
        this.moveWorker.Board = new Chessboard(12);
        Assert.False(this.moveWorker.Board.Insert(Constants.WhiteBishopIdentifier, square));
    }

    /// <summary>
    /// Test that the queen can move both straight and diagnoally.
    /// </summary>
    [Fact]
    public void QueenCanMoveCorrectly()
    {
        this.moveWorker.Board = new Chessboard(6,5);
        
        this.moveWorker.InsertOnBoard(Piece.Queen(PieceClassifier.WHITE), "c4");
        this.moveWorker.Move("c4a6");
        this.moveWorker.Move("a6a1");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier,  this.moveWorker.Board.GetPieceIdentifier("c4"));
        Assert.Equal(Constants.WhiteQueenIdentifier,        this.moveWorker.Board.GetPieceIdentifier("a1"));

    }

    /// <summary>
    /// Test that a custom piece with a novel movement pattern moves correctly.
    /// </summary>
    [Fact]
    public void PieceWithNovelMovementCanMoveCorrectly()
    {
        moveWorker.Board = new Chessboard(8);

        var patterns = new List<Pattern> {
            new RegularPattern(Constants.North,     1, 3),
            new RegularPattern(Constants.NorthEast, 1, 1),
            new RegularPattern(Constants.SouthEast, 2, 2),
            new RegularPattern(Constants.SouthWest, 1, 3),
            new RegularPattern(Constants.NorthWest, 2, 4),

        };
        var mp = new MovementPattern(patterns);
        Piece piece = new Piece(mp, mp, false, PieceClassifier.WHITE, customPieceNotation);

        this.moveWorker.InsertOnBoard(piece, "c4");

        // Valid moves
        this.moveWorker.Move("c4c5");
        this.moveWorker.Move("c5d6");
        this.moveWorker.Move("d6b4");
        this.moveWorker.Move("b4d2");
        this.moveWorker.Move("d2a5");

        Assert.Equal(customPieceNotation, this.moveWorker.Board.GetPieceIdentifier("a5"));

        // Invalid moves
        this.moveWorker.Move("a5b5");
        this.moveWorker.Move("a5b4");
        this.moveWorker.Move("a5a4");

        Assert.Equal(Constants.UnoccupiedSquareIdentifier, this.moveWorker.Board.GetPieceIdentifier("a4"));
    }

    /// <summary>
    /// Tests that a custom piece can repeat its movement pattern, if allowed.
    /// </summary>
    [Fact]
    public void PieceWithRepeatingMovementPatternCanMoveCorrectly()
    {
        this.moveWorker.Board = new Chessboard(8);

        var patterns = new List<Pattern> {
            new RegularPattern(Constants.North, 1, 8),
            new RegularPattern(Constants.West,  1, 8),
        };
        var mp = new MovementPattern(patterns);
        Piece piece1 = new Piece(mp, mp, false, PieceClassifier.WHITE, 1, customPieceNotation, true);
        var piece2 = Piece.BlackPawn();

        this.moveWorker.InsertOnBoard(piece1, "h4");
        this.moveWorker.InsertOnBoard(piece2, "h6");

        this.moveWorker.Move("h4h7"); // Invalid move
        this.moveWorker.Move("h4c5"); // Valid move

        Assert.Equal(customPieceNotation, this.moveWorker.Board.GetPieceIdentifier("c5"));
        
    }

    /// <summary>
    /// Test that the correct amount of valid moves are found.
    /// </summary>
    [Fact]
    public void GetAllValidMovesReturnsCorrectNumberOfMoves()
    {
        var moves1 = this.moveWorker.GetAllValidMoves(Player.White);
        this.moveWorker.Move("e2e3");
        var moves2 = this.moveWorker.GetAllValidMoves(Player.White);

        this.moveWorker.Board = new Chessboard(8);
        this.moveWorker.InsertOnBoard(Piece.Queen(PieceClassifier.WHITE), "e4");
        var movesQueen = this.moveWorker.GetAllValidMoves(Player.White);

        Assert.Equal(12, moves1.Count);
        Assert.Equal(23, moves2.Count);
        Assert.Equal(27, movesQueen.Count);
    }

    /// <summary>
    /// Test that the correct amount of valid capture moves are found. 
    /// </summary>
    [Fact]
    public void GetAllCapturePatternMovesReturnsCorrectNumberOfMoves()
    {
        Move move = new Move("e2e3", PieceClassifier.WHITE);
        
        var moves1 = this.moveWorker.GetAllCapturePatternMoves(Player.White);
        moveWorker.PerformMove(move);
        var moves2 = this.moveWorker.GetAllCapturePatternMoves(Player.White);

        this.moveWorker.Board = new Chessboard(8);

        this.moveWorker.InsertOnBoard(Piece.Queen(PieceClassifier.WHITE), "e4");
        var movesQueen = this.moveWorker.GetAllCapturePatternMoves(Player.White);

        Assert.Equal(18, moves1.Count);
        Assert.Equal(27, moves2.Count);
        Assert.Equal(27, movesQueen.Count);
    }

    [Fact]
    public void MoveLogCorrectlySavesAllMoves()
    {
        var moves = new List<Move> {
            new Move("h2h3", PieceClassifier.WHITE),
            new Move("h3h4", PieceClassifier.WHITE),
            new Move("h1h3", PieceClassifier.WHITE),
            new Move("h3e3", PieceClassifier.WHITE),
        };

        foreach (var move in moves)
        {
            moveWorker.PerformMove(move);
        }

        var expected = new List<string> { "h2h3", "h3h4", "h1h3", "h3e3" };
        var actual = moveWorker.Movelog.Select(move => move.FromTo).ToList();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MovesLikeRook_capturesLikeBishop()
    {
        var patterns = new List<Pattern> {
            new RegularPattern(Constants.North, 1, 8),
            new RegularPattern(Constants.East,  1, 8),
            new RegularPattern(Constants.South, 1, 8),
            new RegularPattern(Constants.West,  1, 8),
        };

        var capturePatterns = new List<Pattern> {
            new RegularPattern(Constants.NorthEast, 1, 8),
            new RegularPattern(Constants.SouthEast, 1, 8),
            new RegularPattern(Constants.SouthWest, 1, 8),
            new RegularPattern(Constants.NorthWest, 1, 8),
        };

        var mp = new MovementPattern(patterns);
        var cp = new MovementPattern(capturePatterns);
        Piece piece = new Piece(mp, cp, false, PieceClassifier.WHITE, customPieceNotation);

        this.moveWorker.InsertOnBoard(piece, "h1");
        this.moveWorker.Move("h2h3");
        this.moveWorker.Move("h3h4");
        this.moveWorker.Move("h1h3");
        
        this.moveWorker.Move("h3e6"); // Invalid move
        this.moveWorker.Move("h3d7"); // Valid move

        Assert.Equal(customPieceNotation, this.moveWorker.Board.GetPieceIdentifier("d7"));

    }

    [Fact]
    public void MoveLikeBishop_captureLikeKnight()
    {
        var patterns = new List<Pattern> {
            new RegularPattern(Constants.NorthEast, 1, 8),
            new RegularPattern(Constants.SouthEast, 1, 8),
            new RegularPattern(Constants.SouthWest, 1, 8),
            new RegularPattern(Constants.NorthWest, 1, 8),
        };

        var capturePattern = new List<Pattern> {
            new JumpPattern( 1, 2),
            new JumpPattern( 2, 1),
            new JumpPattern( 1,-2),
            new JumpPattern( 2,-1),
            new JumpPattern(-1, 2),
            new JumpPattern(-2, 1),
            new JumpPattern(-1,-2),
            new JumpPattern(-2,-1),
        };

        var mp = new MovementPattern(patterns);
        var cp = new MovementPattern(capturePattern);
        Piece piece = new Piece(mp, cp, false, PieceClassifier.WHITE, customPieceNotation);

        this.moveWorker.InsertOnBoard(piece, "h1");
        
        this.moveWorker.Move("h1g3"); // Invalid move
        this.moveWorker.Move("g2g3"); // Valid move
        this.moveWorker.Move("h1b7"); // Invalid move
        this.moveWorker.Move("h1c6"); // Valid move
        this.moveWorker.Move("c6d8"); // Valid move

        Assert.Equal(customPieceNotation, this.moveWorker.Board.GetPieceIdentifier("d8"));

    }

    [Fact]
    public void JumpCapturePattern()
    {
        this.moveWorker.Board = new Chessboard(8);

        var patterns = new List<Pattern> {
            new JumpPattern( 1, 2),
            new JumpPattern( 2, 1),
            new JumpPattern( 1,-2),
            new JumpPattern( 2,-1),
            new JumpPattern(-1, 2),
            new JumpPattern(-2, 1),
            new JumpPattern(-1,-2),
            new JumpPattern(-2,-1),
        };
        
        var capturePatterns = new List<Pattern> {
            new JumpPattern( 3, 1),
            new JumpPattern( 1, 3),
            new JumpPattern(-1, 3),
            new JumpPattern(-3, 1),
        };
        
        var mp = new MovementPattern(patterns);
        var cp = new  MovementPattern(capturePatterns);
        Piece piece1 = new Piece(mp, cp, false, PieceClassifier.WHITE , customPieceNotation);
        Piece piece2 = Piece.BlackPawn();
        
        this.moveWorker.InsertOnBoard(piece1, "d4");
        this.moveWorker.InsertOnBoard(piece2, "e7");

        this.moveWorker.Move("d4e7");

        Assert.Equal(customPieceNotation, this.moveWorker.Board.GetPieceIdentifier("e7"));
    }

    [Fact]
    public void perftTestThreeMoves()
    {
       this.perft.PerftTest (3, Player.White);
       Assert.Equal(8902, perft.Nodes);
    }

    [Fact (Skip = "Takes too long")]
    public void perftTestFiveMoves()
    {
       this.perft.PerftTest( 5, Player.White);
       Assert.Equal(4865609, perft.Nodes);
    }

    
}
