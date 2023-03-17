﻿using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;
/// <summary>
/// Represents a special move that can be performed on the board. (examples: castling, en passant, double pawn move...).
/// _actions is a list of actions that will be performed when the Move is performed.
/// _fromTo is a pair of coordinates (e.g. "e2e4) that represent which piece performs the move and where to click to perform the move respectively.
/// </summary>
public class Move
{
    private readonly IEnumerable<IAction> _actions;
    private readonly string _fromTo;

    public string FromTo => _fromTo;
    public readonly PieceClassifier PieceClassifier;

    /// <summary>
    /// Constructor that takes a list of actions and a string fromTo.
    /// </summary>
    /// <param name="actions">The list of actions that the move performs.</param>
    /// <param name="fromTo">A pair of coordinates, the position of the performing piece and where it ends up.</param>
    public Move(IEnumerable<IAction> actions, string fromTo, PieceClassifier pieceClassifier)
    {
        _actions = actions;
        _fromTo = fromTo;
        PieceClassifier = pieceClassifier;
    }

    /// <summary>
    /// Constructs a standard move.
    /// </summary>
    /// <param name="fromTo">A pair of coordinates, the position of the piece to be moved and where it ends up.</param>
    public Move(string fromTo, PieceClassifier pieceClassifier)
    {
        _actions = new List<IAction>() { new ActionMovePiece(fromTo) };
        _fromTo = fromTo;
        PieceClassifier = pieceClassifier;
    }

    /// <summary>
    /// Performs all actions in the internal list _actions on the given moveWorker.
    /// If any single action fails, the whole move fails.
    /// </summary>
    /// <param name="moveWorker">The board state to perform the actions on.</param>
    /// 
    /// <returns>A GameEvent that determines whether the move succeeded or was invalid.</returns>
    /// 
    public GameEvent Perform(MoveWorker moveWorker)
    {
        var fromTo = MoveWorker.ParseMove(_fromTo);
        if (fromTo == null) throw new ArgumentException("Move needs to contain proper fromTo coordinate, supplied fromTo coordinate: " + _fromTo);
        
        foreach (var action in _actions)
        {
            GameEvent gameEvent = action.Perform(moveWorker, fromTo.Item1);
            if(gameEvent == GameEvent.InvalidMove)
                return GameEvent.InvalidMove;
        }

        moveWorker.AddMove(this);
        return GameEvent.MoveSucceeded;
    }

}