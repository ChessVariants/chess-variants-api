using ChessVariantsLogic.Rules.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ChessVariantsLogic;

namespace ChessVariantsLogic.Tests;

public class ChessEngineTests : IDisposable
{
    private static List<Piece> pieces= new List<Piece>();
    private Game game;
    private Game antiChessGame;
    private static Chessboard chessboard = new Chessboard(8,8);
    private MoveWorker moveWorker;
    private static PieceValue pieceValue = new PieceValue(Piece.AllStandardPieces(), chessboard);
    private NegaMax negaMax= new NegaMax(pieceValue);

    public ChessEngineTests()
    {
        this.moveWorker = new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces());
        this.game = GameFactory.StandardChess();
        this.antiChessGame = GameFactory.AntiChess();
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
        Move bestMove = negaMax.FindBestMove(3,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(from, bestMove.From);
    }

    [Fact]
    public void negaMaxTakesFreePiece()
    {
        //HeatMap asdfasdfasdf = new HeatMap(8,8);
        game.MoveWorker.InsertOnBoard(Piece.Rook(PieceClassifier.BLACK), "d5");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact ]
    public void negaMaxDoesNotTakeDefendedPawnWithKnigt()
    {
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "a3");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "d6");
        game.MoveWorker.InsertOnBoard(Piece.Knight(PieceClassifier.WHITE), "b2");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
        Assert.NotEqual(moveFreePiece, bestMove.To);
    }

    [Fact ]
    public void negaMaxCheckMate()
    {
        Move move1 = new Move("g2g4", PieceClassifier.WHITE);
        Move move2 = new Move("e7e5", PieceClassifier.BLACK);
        Move move3 = new Move("f2f3", PieceClassifier.WHITE);
      
       game.MoveWorker.PerformMove(move1);
       game.MoveWorker.PerformMove(move2);
       game.MoveWorker.PerformMove(move3);
        string moveFreePiece = "h4";
        Move bestMove = negaMax.FindBestMove(3,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact ]
    public void negaMaxDefendsCheckMate()
    {
        Move move1 = new Move("g2g4", PieceClassifier.WHITE);
        Move move2 = new Move("e7e5", PieceClassifier.BLACK);
        Move move3 = new Move("f2f3", PieceClassifier.WHITE);
      
       game.MoveWorker.PerformMove(move1);
       game.MoveWorker.PerformMove(move2);
       game.MoveWorker.PerformMove(move3);
        string moveFreePiece = "d3";
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact]
    public void negaMaxDefendsCheckMateBug()
    {
        Chessboard empty = new Chessboard(8,8);
        game.MoveWorker.Board = empty;
        game.MoveWorker.InsertOnBoard(Piece.WhitePawn(), "a2");
        game.MoveWorker.InsertOnBoard(Piece.WhitePawn(), "b2");
        game.MoveWorker.InsertOnBoard(Piece.King(PieceClassifier.WHITE), "a1");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.WHITE), "c2");
        game.MoveWorker.InsertOnBoard(Piece.Queen(PieceClassifier.BLACK), "e1");
        
        string moveFreePiece = "c2";
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.From);
    }

    [Fact]
    public void negaMaxForcedCheckMateBug()
    {
        Chessboard empty = new Chessboard(8,8);
        game.MoveWorker.Board = empty;
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "a7");
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "b7");
        game.MoveWorker.InsertOnBoard(Piece.King(PieceClassifier.BLACK), "a8");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "c7");
        game.MoveWorker.InsertOnBoard(Piece.Queen(PieceClassifier.WHITE), "e8");
        
        string moveFreePiece = "c7";
        Move bestMove = negaMax.FindBestMove(3,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.From);
    }

    [Fact ]
    public void antiChess()
    {
        Move move1 = new Move("e2e4", PieceClassifier.WHITE);
       /* Move move2 = new Move("b7b5", PieceClassifier.BLACK);
        Move move3 = new Move("f1b5", PieceClassifier.WHITE);*/
      
       antiChessGame.MoveWorker.PerformMove(move1);
       /*game.MoveWorker.PerformMove(move2);
       game.MoveWorker.PerformMove(move3);*/
        string moveFreePiece = "b7b5";
        Move bestMove = negaMax.FindBestMove(3,antiChessGame, Player.Black, ScoreVariant.AntiChess);
        Assert.Equal(moveFreePiece, bestMove.FromTo);
    }
}