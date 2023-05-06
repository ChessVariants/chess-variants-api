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
        Move bestMove = negaMax.FindBestMove(3,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(from, bestMove.From);
    }

    [Fact]
    public void NegaMaxTakesFreePiece()
    {
        
        game.MoveWorker.InsertOnBoard(Piece.Rook(PieceClassifier.BLACK), "a3");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact ]
    public void NegaMaxDoesNotTakeDefendedPawnWithKnigt()
    {
        game.MoveWorker.InsertOnBoard(Piece.Knight(PieceClassifier.BLACK), "a3");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "d6");
        game.MoveWorker.InsertOnBoard(Piece.Knight(PieceClassifier.WHITE), "b2");
        string moveFreePiece = "a3";
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
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
        Move bestMove = negaMax.FindBestMove(3,game, Player.Black, ScoreVariant.RegularChess);
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
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
        //Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact]
    public void NegaMaxDefendsCheckMateBug()
    {
        Chessboard empty = new Chessboard(8,8);
        game.MoveWorker.Board = empty;
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "e5");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.WHITE), "c3");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.WHITE), "g3");
        game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "f6");
        //game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "c7");
        
        string moveFreePiece = "c2";
        Move bestMove = negaMax.FindBestMove(3,game, Player.White, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.To);
    }

    [Fact]
    public void NegaMaxForcedCheckMateBug()
    {
        Chessboard empty = new Chessboard(8,8);
        game.MoveWorker.Board = empty;
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "e5");
      
        
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "d6");
        game.MoveWorker.InsertOnBoard(Piece.BlackPawn(), "f6");
        game.MoveWorker.InsertOnBoard(Piece.Knight(PieceClassifier.WHITE),"d3");
        //game.MakeMove("c3e5",Player.White);
        //game.MoveWorker.InsertOnBoard(Piece.Bishop(PieceClassifier.BLACK), "c7");
        
        string moveFreePiece = "c2";
        
        
        Move bestMove = negaMax.FindBestMove(2,game, Player.White, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.FromTo);
    }

    [Fact]
    public void GameTest()
    {
        game.MakeMove("e2e4", Player.White);
        game.MakeMove("e7e5", Player.Black);   
        game.MakeMove("g1f3", Player.White);
        game.MakeMove("d7d5", Player.Black);   
        game.MakeMove("f3e5", Player.White);
        game.MakeMove("d5e4", Player.Black);   
        game.MakeMove("b1c3", Player.White);
        game.MakeMove("d8d4", Player.Black);
        game.MakeMove("c3b5", Player.White);
        game.MakeMove("d4e5", Player.Black);
        game.MakeMove("f1c4", Player.White);
        game.MakeMove("c7c6", Player.Black);
        game.MakeMove("b5c3", Player.White);
        game.MakeMove("e5d4", Player.Black);
        game.MakeMove("d1e2", Player.White);
        game.MakeMove("c8f5", Player.Black);
        game.MakeMove("d2d3", Player.White);
        /*game.MakeMove("g8f6", Player.Black);
        game.MakeMove("c1g5", Player.White);
        game.MakeMove("f8b4", Player.Black);
        game.MakeMove("a2a3", Player.White);
        game.MakeMove("b4c3", Player.Black);
        game.MakeMove("b2c3", Player.White);
        game.MakeMove("d4c3", Player.Black);
        game.MakeMove("e1f1", Player.White);
        game.MakeMove("c3a1", Player.Black);
        game.MakeMove("e2e1", Player.White);
        game.MakeMove("a1e1", Player.Black);
        game.MakeMove("f1e1", Player.White);
        game.MakeMove("f6d5", Player.Black);
        game.MakeMove("d3d4", Player.White);
        game.MakeMove("f7f6", Player.Black);
        game.MakeMove("g5h4", Player.White);
        game.MakeMove("g7g5", Player.Black);
        game.MakeMove("h4g3", Player.White);
        game.MakeMove("b8d7", Player.Black);
        game.MakeMove("e1d2", Player.White);
        game.MakeMove("e8f7", Player.Black);
        game.MakeMove("h2h3", Player.White);
        game.MakeMove("f7e6", Player.Black);
        game.MakeMove("h3h4", Player.White);
        game.MakeMove("g5h4", Player.Black);
        game.MakeMove("h1h4", Player.White);
        game.MakeMove("h8g8", Player.Black);
        game.MakeMove("h4h7", Player.White);
        game.MakeMove("f5h7", Player.Black);
        game.MakeMove("a3a4", Player.White);
        game.MakeMove("h7f5", Player.Black);
        game.MakeMove("c2c3", Player.White);
        game.MakeMove("d7b6", Player.Black);
        game.MakeMove("c4b3", Player.White);
        game.MakeMove("e4e3", Player.Black);
        game.MakeMove("d2e2", Player.White);
        game.MakeMove("f5e4", Player.Black);
        game.MakeMove("b3d5", Player.White);
        game.MakeMove("b6d5", Player.Black);
        game.MakeMove("c3c4", Player.White);
        game.MakeMove("d5e7", Player.Black);
        game.MakeMove("e2f1", Player.White);
        game.MakeMove("e7f5", Player.Black);
        game.MakeMove("g3c7", Player.White);
        game.MakeMove("f5d4", Player.Black);
        game.MakeMove("f2e3", Player.White);
        game.MakeMove("e4d3", Player.Black);
        game.MakeMove("f1f2", Player.White);
        game.MakeMove("d4f5", Player.Black);
        game.MakeMove("a4a5", Player.White);
        game.MakeMove("d3c4", Player.Black);
        game.MakeMove("f2f3", Player.White);
        game.MakeMove("e6d5", Player.Black);
        game.MakeMove("g2g4", Player.White);
        game.MakeMove("f5h4", Player.Black);
        game.MakeMove("f3f4", Player.White);
        game.MakeMove("h4g2", Player.Black);
        game.MakeMove("f4f3", Player.White);
        game.MakeMove("g2h4", Player.Black);
        game.MakeMove("f3f4", Player.White);*/

        string moveFreePiece = "c7";
        Move bestMove = negaMax.FindBestMove(2,game, Player.Black, ScoreVariant.RegularChess);
        Assert.Equal(moveFreePiece, bestMove.FromTo);
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
