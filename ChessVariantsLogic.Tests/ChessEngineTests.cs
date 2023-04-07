using ChessVariantsLogic.Rules.Moves;
using System;
using System.Collections.Generic;
using Xunit;
using ChessVariantsLogic.Engine;

namespace ChessVariantsLogic.Tests;

public class ChessEngineTests : IDisposable
{
    private static List<Piece> pieces = new List<Piece>();
    private Game game;
    private MoveWorker moveWorker;
    private static PieceValue pieceValue = new PieceValue(Piece.AllStandardPieces());
    private NegaMax negaMax = new NegaMax(pieceValue);

    public ChessEngineTests()
    {
        this.moveWorker = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        this.game = GameFactory.StandardChess();
    }


    public void Dispose()
    {
        this.moveWorker = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void negaMaxDoesNotLetOpponentTakeFreePiece()
    {
        game.MoveWorker.InsertOnBoard(Piece.Rook(PieceClassifier.BLACK), "a3");
        string from = "a3";
        Move bestMove = negaMax.findBestMove(2, game, Player.Black);
        Assert.Equal(from, bestMove.From);
    }

    [Fact]
    public void negaMaxTakesFreePiece()
    {
        game.MoveWorker.InsertOnBoard(Piece.Rook(PieceClassifier.BLACK), "a3");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.findBestMove(3, game, Player.White);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact]
    public void negaMaxDoesNotTakeDefendedPawnWithKnigt()
    {
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "a3");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "d6");
        game.MoveWorker.InsertOnBoard(Piece.Knight(PieceClassifier.WHITE), "b2");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.findBestMove(3, game, Player.White);
        Assert.NotEqual(moveFreePiece, bestMove.To);
    }
}
