using ChessVariantsLogic;
using ChessVariantsLogic.Predicates;
using System;
using System.Numerics;

namespace ChessVariantsLogic.Predicates;

public class Attacked : IPredicate
{
    private readonly BoardState _boardState;
    private readonly string _pieceIdentifier;

    public Attacked(BoardState boardState, string pieceIdentifier)
	{
        _boardState = boardState;
        _pieceIdentifier = pieceIdentifier;
    }

    public bool Evaluate(Chessboard thisBoardState, Chessboard nextBoardState)
    {
        Chessboard board = _boardState == BoardState.NEXT ? nextBoardState : thisBoardState;

        var attacked = Utils.PieceAttacked(board, _pieceIdentifier);
        return attacked;
    }

}
public enum BoardState
{
    THIS, NEXT
}