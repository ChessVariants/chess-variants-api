namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Export;
using static ChessVariantsLogic.Game;


using System;
public class Perft : Game
{
    
    
    private int nodes = 0;
    private readonly MoveWorker _moveWorker;
    private Player _playerTurn;
    private int _playerMovesRemaining;
    private readonly int _movesPerTurn;
    private readonly RuleSet _blackRules;
    private readonly RuleSet _whiteRules;

    public Perft(MoveWorker moveWorker, Player playerToStart, int movesPerTurn, RuleSet whiteRules, RuleSet blackRules) : base(moveWorker, playerToStart, movesPerTurn, whiteRules, blackRules)
    {
        _moveWorker = moveWorker;
        _playerTurn = playerToStart;
        _movesPerTurn = _playerMovesRemaining = movesPerTurn;
        _whiteRules = whiteRules;
        _blackRules = blackRules;
    }

    public int Nodes
    {
        get{ return this.nodes; }
    }
    public void perftTest(int depth, Player turn)
    {
        
        if (depth == 0)
        {
            nodes++;
            return;
        }
        IEnumerable<Move> validMoves;
        
        if (turn == Player.White)
        {
            validMoves = _whiteRules.ApplyMoveRule(_moveWorker, Player.White);
            turn = Player.Black;
        }
        else
        {
            validMoves = _blackRules.ApplyMoveRule(_moveWorker, Player.Black);
            turn = Player.White;
        }

        foreach (var move in validMoves)
        {
            var boardTmp = _moveWorker.Board.CopyBoard();
            _moveWorker.StateLog.Push(boardTmp);
            move.Perform(_moveWorker);
            perftTest(depth - 1, turn);
            _moveWorker.undoMove();
        }
        return;
    }
}