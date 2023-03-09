using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;

using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using Predicates;
using System;

/// <summary>
/// A MoveData object represents a special move that can be performed by all pieces with the given _pieceIdentifier.
/// The core idea is that you have a _predicate and a list of _actions. The predicate must hold for the actions to be performed.
/// A RuleSet is supplied with a list of MoveData. When appliedMoveRule is performed, it will loop through all MoveData
/// and retrieve all SpecialMoves from the GetValidMoves method.
/// </summary>
public class MoveData
{
    private readonly IEnumerable<IAction> _actions;
    private readonly IPredicate _predicate;
    private readonly string _pieceIdentifier;
    private readonly IPosition _to;

    public IEnumerable<IAction> Actions => _actions;
    public string PieceIdentifier => _pieceIdentifier;

    public MoveData(IEnumerable<IAction> actions, IPredicate predicate, string pieceIdentifier, IPosition to)
    {
        _actions = actions;
        _predicate = predicate;
        _pieceIdentifier = pieceIdentifier;
        _to = to;
    }

    /// <summary>
    /// Constructs a SpecialMove for each piece with the given _pieceIdentifier.
    /// The SpecialMove is constructed from the internal _actions, _to and that pieces position.
    /// 
    /// <para>Loops through all pieces of the given _pieceIdentifier.</para>
    /// <para>Then constructs a SpecialMove object with the internal _actions list and the calculated move coordinate.</para>
    /// <para>Performs the SpecialMove on a copied board</para>
    /// <para>Evaluates the internal _predicate, the supplied moveRule and check if the SpecialMove was performed successfully</para>
    /// <para>If the above is true, it is added to the list of SpecialMove</para>
    /// </summary>
    /// <param name="thisBoard">The current board state used to construct the SpecialMove list from</param>
    /// <param name="moveRule">The moveRule of the RuleSet retrieving the valid moves</param>
    /// 
    /// <returns>A list of special moves that can be performed on the given board state.</returns>
    /// 
    public IEnumerable<Move> GetValidMoves(MoveWorker thisBoard, IPredicate moveRule)
    {
        List<string> positions = (List<string>) Utils.FindPiecesOfType(thisBoard, _pieceIdentifier);
        HashSet<Move> moves = new HashSet<Move>();

        foreach(string from in positions)
        {
            string? to = _to.GetPosition(thisBoard, from);
            if (to == null) continue;

            Move move = new Move(_actions, from + to);
            MoveWorker futureBoard = thisBoard.CopyBoardState();
            GameEvent result = move.Perform(futureBoard);
            
            BoardTransition transition = new BoardTransition(thisBoard, futureBoard, from, to);
            if(_predicate.Evaluate(transition) && moveRule.Evaluate(transition) && result != GameEvent.InvalidMove)
            {
                moves.Add(move);
            }

        }

        return moves;
    }

    public static MoveData CastleMove(Player player, bool kingSide, bool kingCanMoveThroughChecks)
    {
        string KingIdentifier = player == Player.White ? Constants.WhiteKingIdentifier : Constants.BlackKingIdentifier;
        string RookIdentifier = player == Player.White ? Constants.WhiteRookIdentifier : Constants.BlackRookIdentifier;
        Player attacker = player == Player.White ? Player.Black : Player.White;
        
        IPredicate castlePredicate;

        string rank = player == Player.White ? "1" : "8";
        string kingFromFile = "e";
        string kingToFile = kingSide ? "g" : "c";
        string rookFromFile = kingSide ? "h" : "a";
        string rookToFile = kingSide ? "f" : "d";
        int kingSideMultiplier = kingSide ? 1 : -1;

        int relativeRookX = kingSide ? 3 : -4;

        IPosition kingPosition = new PositionRelative(row: 0, col: 0);
        IPosition squareBetween = new PositionRelative(row: 0, col: kingSideMultiplier);
        IPosition rookPosition = new PositionRelative(row: 0, col: relativeRookX);

        IPredicate kingCheckedThisState = new Attacked(BoardState.THIS, KingIdentifier);
        IPredicate kingCheckedNextState = new Attacked(BoardState.NEXT, KingIdentifier);


        IPredicate squareBetweenAttacked = new SquareAttacked(squareBetween, BoardState.THIS, attacker);

        IPredicate kingMovesThroughCheck = kingCheckedThisState | squareBetweenAttacked | kingCheckedNextState;

        IPredicate squareEmpty1 = new PieceAt(Constants.UnoccupiedSquareIdentifier, new PositionRelative(row: 0, col: 1 * kingSideMultiplier), BoardState.THIS);
        IPredicate squareEmpty2 = new PieceAt(Constants.UnoccupiedSquareIdentifier, new PositionRelative(row: 0, col: 2 * kingSideMultiplier), BoardState.THIS);
        IPredicate squareEmpty3 = new PieceAt(Constants.UnoccupiedSquareIdentifier, new PositionRelative(row: 0, col: 3 * kingSideMultiplier), BoardState.THIS);

        IPredicate kingHasNotMoved = new TimesMoved(kingPosition, Comparator.EQUALS, 0);
        IPredicate rookHasNotMoved = new TimesMoved(rookPosition, Comparator.EQUALS, 0);

        IPredicate kingAtSquare = new PieceAt(KingIdentifier, kingPosition, BoardState.THIS);
        IPredicate rookAtSquare = new PieceAt(RookIdentifier, rookPosition, BoardState.THIS);

        castlePredicate = squareEmpty1 & squareEmpty2 & kingAtSquare & rookAtSquare;

        // TimesMoved not yet implemented but we should also check if the king and rook hasn't moved yet
        // castlePredicate &= !kingHasNotMoved & !rookHasNotMoved;

        if (!kingSide)
            castlePredicate &= squareEmpty3;

        if (!kingCanMoveThroughChecks)
        {
            castlePredicate &= !kingMovesThroughCheck;
        }

        IPosition kingFromPos = new PositionAbsolute(kingFromFile + rank);
        IPosition kingToPos = new PositionAbsolute(kingToFile + rank);

        IPosition rookFromPos = new PositionAbsolute(rookFromFile + rank);
        IPosition rookToPos = new PositionAbsolute(rookToFile + rank);

        IEnumerable<IAction> castleActions = new List<IAction>
        {
            new ActionMovePiece(kingFromPos, kingToPos),
            new ActionMovePiece(rookFromPos, rookToPos)
        };

        return new MoveData(castleActions, castlePredicate, KingIdentifier, rookPosition);
    }

    
    
    public static MoveData PawnDoubleMove(Player player)
    {
        int playerMultiplier = player == Player.White ? -1 : 1;
        string PawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        IPosition thisPawnPosition = new PositionRelative(row: 0, col: 0);
        IPosition forwardPosition1 = new PositionRelative(row: 1 * playerMultiplier, col: 0);
        IPosition forwardPosition2 = new PositionRelative(row: 2 * playerMultiplier, col: 0);

        IPredicate hasNotMoved = new TimesMoved(thisPawnPosition, Comparator.EQUALS, 0);
        IPredicate targetSquareEmpty1 = new PieceAt(Constants.UnoccupiedSquareIdentifier, forwardPosition1, BoardState.THIS);
        IPredicate targetSquareEmpty2 = new PieceAt(Constants.UnoccupiedSquareIdentifier, forwardPosition2, BoardState.THIS);

        IEnumerable<IAction> actions = new List<IAction>
        {
            new ActionMovePiece(forwardPosition2)
        };
        //Times moved not yet implemented but we need to check that pawn hasn't moved before
        return new MoveData(actions, /*hasNotMoved &*/ targetSquareEmpty1 & targetSquareEmpty2, PawnIdentifier, forwardPosition2);
    }



    public static MoveData EnPassantMove(Player player, bool right)
    {
        int playerMultiplier = player == Player.White ? -1 : 1;
        int sideMultiplier = right ? 1 : -1;
        string PawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        string OpponentPawnIdentifier = player == Player.White ? Constants.BlackPawnIdentifier : Constants.WhitePawnIdentifier;

        IPosition finalPosition = new PositionRelative(row: playerMultiplier, col: sideMultiplier);
        IPosition enemyPawnPosition = new PositionRelative(row: 0, col: sideMultiplier);

        IPredicate enemyPawnNextTo = new PieceAt(OpponentPawnIdentifier, enemyPawnPosition, BoardState.THIS);
        IPredicate targetSquareEmpty = new PieceAt(Constants.UnoccupiedSquareIdentifier, finalPosition, BoardState.THIS);
        IPredicate enemyPawnHasMovedOnce = new TimesMoved(enemyPawnPosition, Comparator.EQUALS, 1);

        IEnumerable<IAction> actions = new List<IAction>
        {
            new ActionMovePiece(finalPosition),
            new ActionDeletePiece(enemyPawnPosition)
        };

        // Times moved not yet implemented but we need to check that the enemy pawn has only moved once
        // We also need to check that we're on the correct rank
        return new MoveData(actions, enemyPawnNextTo & targetSquareEmpty/* & enemyPawnHasMoved*/, PawnIdentifier, finalPosition);
    }

}