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
    private Game antiChessGame;
    private MoveWorker moveWorker;
    private static Chessboard chessboard = new Chessboard(8,8);
    
    //private static PieceValue pieceValue = new PieceValue(Piece.AllStandardPieces());
    private NegaMax negaMax = new NegaMax(Piece.AllStandardPieces(), chessboard);

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
        Move bestMove = negaMax.FindBestMove(2,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(from, bestMove.From);
    }

    [Fact]
    public void NegaMaxTakesFreePiece()
    {
        
        game.MoveWorker.InsertOnBoard(Piece.Rook(PieceClassifier.BLACK), "a3");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.FindBestMove(2,game, Player.White, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact ]
    public void NegaMaxDoesNotTakeDefendedPawnWithKnigt()
    {
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "a3");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "d6");
        game.MoveWorker.InsertOnBoard(Piece.Knight(PieceClassifier.WHITE), "b2");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.FindBestMove(2,game, Player.White, ScoreVariant.RegularChess);
        Assert.NotEqual(moveFreePiece, bestMove.To);
    }

    [Fact ]
    public void NegaMaxCheckMate()
    {
        Move move1 = new Move("g2g4", Piece.WhitePawn());
        Move move2 = new Move("e7e5", Piece.BlackPawn());
        Move move3 = new Move("f2f3", Piece.WhitePawn());
      
       game.MoveWorker.PerformMove(move1);
       game.MoveWorker.PerformMove(move2);
       game.MoveWorker.PerformMove(move3);
        string moveFreePiece = "h4";
        Move bestMove = negaMax.FindBestMove(2,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact ]
    public void NegaMaxDefendsCheckMate()
    {
        Move move1 = new Move("g2g4", Piece.WhitePawn());
        Move move2 = new Move("e7e5", Piece.BlackPawn());
        Move move3 = new Move("f2f3", Piece.WhitePawn());
      
       game.MoveWorker.PerformMove(move1);
       game.MoveWorker.PerformMove(move2);
       game.MoveWorker.PerformMove(move3);
        string moveFreePiece = "d3";
        Move bestMove = negaMax.FindBestMove(2,game, Player.White, ScoreVariant.RegularChess);
        //Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact]
    public void NegaMaxDefendsCheckMateBug()
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
    public void NegaMaxForcedCheckMateBug()
    {
        Chessboard empty = new Chessboard(8,8);
        game.MoveWorker.Board = empty;
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "a7");
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "b7");
        game.MoveWorker.InsertOnBoard(Piece.King(PieceClassifier.BLACK), "a8");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "d7");
        game.MoveWorker.InsertOnBoard(Piece.Queen(PieceClassifier.WHITE), "h8");
        
        string moveFreePiece = "d7";
        Move bestMove = negaMax.FindBestMove(3,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.From);
    }

    [Fact]
    public void AntiChessBugg()
    {
        Chessboard empty = new Chessboard(8,8);
        antiChessGame.MoveWorker.Board = empty;

        antiChessGame.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "e3");
        antiChessGame.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "f3");
        antiChessGame.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "h4");
        antiChessGame.MoveWorker.InsertOnBoard(Piece.Knight(PieceClassifier.WHITE), "e4");
        
        

        var bestMove = negaMax.FindBestMove(3,antiChessGame, Player.Black, ScoreVariant.AntiChess);
        Assert.Equal("f3f2", bestMove.FromTo);
    }

    [Fact ]
    public void AntiChessTest()
    {
        
        
        antiChessGame.MakeMove("h2h4", Player.White);
        antiChessGame.MakeMove("b8a6", Player.Black);
        antiChessGame.MakeMove("h4h5", Player.White);
        antiChessGame.MakeMove("a6b8", Player.Black);
        antiChessGame.MakeMove("h5h6", Player.White);
        var legalMoves = antiChessGame.LegalMoves;
        
        
    }
}
