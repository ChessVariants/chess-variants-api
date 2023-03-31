using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;

using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using Predicates;

/// <summary>
/// A MoveTemplate object represents a special move that can be performed by all pieces with the given _pieceIdentifier.
/// The core idea is that you have a _predicate and a list of _actions. The predicate must hold for the actions to be performed.
/// </summary>
public class MoveTemplate
{
    private readonly List<Action> _actions;
    private readonly IPredicate _predicate;
    private readonly string _pieceIdentifier;
    private readonly IPosition _to;
    
    public List<Action> Actions => _actions;
    public string PieceIdentifier => _pieceIdentifier;

    public MoveTemplate(List<Action> actions, IPredicate predicate, string pieceIdentifier, IPosition to)
    {
        _actions = actions;
        _predicate = predicate;
        _pieceIdentifier = pieceIdentifier;
        _to = to;
    }

    /// <summary>
    /// Constructs a Move for each piece with the given _pieceIdentifier.
    /// The Move is constructed from the internal _actions, _to and that piece's position.
    /// </summary>
    /// <param name="moveWorker">The current board state used to construct the Move list from.</param>
    /// <param name="moveRule">The moveRule of the RuleSet retrieving the valid moves.</param>
    /// <param name="events">The events of the RuleSet retrieving the valid moves.</param>
    /// <returns>A list of special moves that can be performed on the given board state.</returns>
    /// 
    public ISet<Move> GetValidMoves(MoveWorker moveWorker, IPredicate moveRule, ISet<Event> events)
    {
        List<string> positions = (List<string>)Utils.FindPiecesOfType(moveWorker, _pieceIdentifier);
        PieceClassifier pieceClassifier = moveWorker.GetPieceClassifier(_pieceIdentifier);
        ISet<Move> moves = new HashSet<Move>();

        foreach (string from in positions)
        {
            string? to = _to.GetPosition(moveWorker, from);
            if (to == null) continue;

            Move move = new Move(_actions, from + to, moveWorker.GetPieceFromIdentifier(moveWorker.Board.GetPieceIdentifier(from)));

            BoardTransition transition = new BoardTransition(moveWorker, move, events);
            if (_predicate.Evaluate(transition) && moveRule.Evaluate(transition) && transition.IsValid())
            {
                moves.Add(move);
            }
        }

        return moves;
    }
    /// <summary>
    /// Returns true if there is at least on move that can be performed with this template.
    /// </summary>
    /// <param name="moveWorker">The MoveWorker to check if there are valid moves on.</param>
    /// <param name="moveRule">The moveRule of the RuleSet checking if there are valid moves.</param>
    /// <param name="events">The events of the RuleSet checking if there are valid moves.</param>
    /// <returns></returns>
    public bool HasValidMoves(MoveWorker moveWorker, IPredicate moveRule, ISet<Event> events)
    {
        List<string> positions = (List<string>)Utils.FindPiecesOfType(moveWorker, _pieceIdentifier);
        PieceClassifier pieceClassifier = moveWorker.GetPieceClassifier(_pieceIdentifier);

        foreach (string from in positions)
        {
            string? to = _to.GetPosition(moveWorker, from);
            if (to == null) continue;

            Move move = new Move(_actions, from + to, moveWorker.GetPieceFromIdentifier(moveWorker.Board.GetPieceIdentifier(from)));

            BoardTransition transition = new BoardTransition(moveWorker, move, events);
            if (_predicate.Evaluate(transition) && moveRule.Evaluate(transition) && transition.IsValid())
            {
                return true;
            }
        }

        return false;
    }

    public static MoveTemplate CastleMove(Player player, bool kingSide, bool kingCanMoveThroughChecks)
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

        IPredicate kingHasMoved = new HasMoved(kingPosition);
        IPredicate rookHasMoved = new HasMoved(rookPosition);

        IPredicate kingAtSquare = new PieceAt(KingIdentifier, kingPosition, BoardState.THIS);
        IPredicate rookAtSquare = new PieceAt(RookIdentifier, rookPosition, BoardState.THIS);

        castlePredicate = squareEmpty1 & squareEmpty2 & kingAtSquare & rookAtSquare & !kingHasMoved & !rookHasMoved; ;

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

        List<Action> castleActions = new List<Action>
        {
            new ActionMovePiece(kingFromPos, kingToPos),
            new ActionMovePiece(rookFromPos, rookToPos)
        };

        return new MoveTemplate(castleActions, castlePredicate, KingIdentifier, kingToPos);
    }

    
    
    public static MoveTemplate PawnDoubleMove(Player player)
    {
        int playerMultiplier = player == Player.White ? -1 : 1;
        string PawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        IPosition thisPawnPosition = new PositionRelative(row: 0, col: 0);
        IPosition forwardPosition1 = new PositionRelative(row: 1 * playerMultiplier, col: 0);
        IPosition forwardPosition2 = new PositionRelative(row: 2 * playerMultiplier, col: 0);

        IPredicate hasMoved = new HasMoved(thisPawnPosition);
        IPredicate targetSquareEmpty1 = new PieceAt(Constants.UnoccupiedSquareIdentifier, forwardPosition1, BoardState.THIS);
        IPredicate targetSquareEmpty2 = new PieceAt(Constants.UnoccupiedSquareIdentifier, forwardPosition2, BoardState.THIS);

        List<Action> actions = new List<Action>
        {
            new ActionMovePiece(forwardPosition2)
        };
        //Times moved not yet implemented but we need to check that pawn hasn't moved before
        return new MoveTemplate(actions, !hasMoved & targetSquareEmpty1 & targetSquareEmpty2, PawnIdentifier, forwardPosition2);
    }



    public static MoveTemplate EnPassantMove(Player player, bool right)
    {
        int playerMultiplier = player == Player.White ? -1 : 1;
        int sideMultiplier = right ? 1 : -1;
        string PawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        string OpponentPawnIdentifier = player == Player.White ? Constants.BlackPawnIdentifier : Constants.WhitePawnIdentifier;

        IPosition finalPosition = new PositionRelative(row: playerMultiplier, col: sideMultiplier);
        IPosition enemyPawnPosition = new PositionRelative(row: 0, col: sideMultiplier);
        IPosition enemyPawnPositionFrom = new PositionRelative(row: 2 * playerMultiplier, col: sideMultiplier);

        IPredicate enemyPawnNextTo = new PieceAt(OpponentPawnIdentifier, enemyPawnPosition, BoardState.THIS);
        IPredicate targetSquareEmpty = new PieceAt(Constants.UnoccupiedSquareIdentifier, finalPosition, BoardState.THIS);
        IPredicate pawnJustDidDoubleMove = new MoveWas(enemyPawnPositionFrom, enemyPawnPosition, MoveState.LAST);

        List<Action> actions = new List<Action>
        {
            new ActionMovePiece(finalPosition),
            new ActionDeletePiece(enemyPawnPosition)
        };

        return new MoveTemplate(actions, enemyPawnNextTo & targetSquareEmpty & pawnJustDidDoubleMove, PawnIdentifier, finalPosition);
    }

}