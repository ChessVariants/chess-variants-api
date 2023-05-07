using ChessVariantsLogic.Export;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChessVariantsLogic.Tests;
public class BoardExporterTests
{
    GameState gameState;

    public BoardExporterTests()
    {
        var moveWorker = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        gameState = GameExporter.ExportGameState(moveWorker, Player.White, moveWorker.GetMoveDict(Player.White));
    }
    
    [Fact]
    public void ExportGameState_ShouldHaveMoves()
    {
        Assert.NotEmpty(gameState.Moves);
    }

    [Fact]
    public void ExportGameState_KnightShouldHave2Moves()
    {
        var knightMove = gameState.Moves.Find(move => move.From == "g1");
        Assert.Equal(2, knightMove!.To.Count());
        Assert.Contains("h3", knightMove!.To);
        Assert.Contains("f3", knightMove!.To);
    }

    [Fact]
    public void ExportGameState_ShouldGenerateBoardAsFollows()
    {
        var correctBoard = new List<string>()
        {
            Constants.BlackRookImage,
            Constants.BlackKnightImage,
            Constants.BlackBishopImage,
            Constants.BlackQueenImage,
            Constants.BlackKingImage,
            Constants.BlackBishopImage,
            Constants.BlackKnightImage,
            Constants.BlackRookImage,
            Constants.BlackPawnImage+"8",
            Constants.UnoccupiedSquareIdentifier+"32",
            Constants.WhitePawnImage+"8",
            Constants.WhiteRookImage,
            Constants.WhiteKnightImage,
            Constants.WhiteBishopImage,
            Constants.WhiteQueenImage,
            Constants.WhiteKingImage,
            Constants.WhiteBishopImage,
            Constants.WhiteKnightImage,
            Constants.WhiteRookImage,
        };

        Assert.Equal(correctBoard, gameState.Board);
    }

    [Fact]
    public void ExportGameState_ShouldHave8x8BoardSize()
    {
        Assert.Equal(8, gameState.BoardSize.Rows);
        Assert.Equal(8, gameState.BoardSize.Cols);
    }

    [Fact]
    public void ExportGameState_BoardShouldHaveThreeElements()
    {
        var mw = new MoveWorker(new Chessboard(10));
        mw.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "e4");
        var gameState = GameExporter.ExportGameState(mw, Player.Black, mw.GetMoveDict(Player.Black));
        Assert.Contains(Constants.BlackBishopImage, gameState.Board);
        Assert.Equal(3, gameState.Board.Count); // 1. All squares leading up to the bishop, the bishop's square, and all squares afterwards
    }

    [Fact]
    public void ExportGameState_BishopShouldHave13Moves()
    {
        var mw = new MoveWorker(new Chessboard(8));
        mw.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "e4");
        var gameState = GameExporter.ExportGameState(mw, Player.Black, mw.GetMoveDict(Player.Black));
        var bishopMove = gameState.Moves.Find(move => move.From == "e4");
        Assert.Equal(13, bishopMove!.To.Count());
    }

    [Fact]
    public void ExportGameState_EmptyBoardShouldHaveNoMoves()
    {
        var mw = new MoveWorker(new Chessboard(8));
        var emptyGameState = GameExporter.ExportGameState(mw, Player.Black, mw.GetMoveDict(Player.Black));
        Assert.Empty(emptyGameState.Moves);
    }

    [Fact]
    public void ExportGameState_EmptyBoardShouldOnlyBeUnoccupied()
    {
        var mw = new MoveWorker(new Chessboard(10));
        var emptyGameState = GameExporter.ExportGameState(mw, Player.Black, mw.GetMoveDict(Player.Black));
        Assert.Equal(Constants.UnoccupiedSquareIdentifier + "100", emptyGameState.Board.First());
        Assert.Single(emptyGameState.Board);
    }
}
