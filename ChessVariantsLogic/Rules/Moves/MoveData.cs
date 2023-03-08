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
    private readonly Tuple<int, int> _relativeTo;

    public IEnumerable<IAction> Actions => _actions;
    public string PieceIdentifier => _pieceIdentifier;

    public MoveData(IEnumerable<IAction> actions, IPredicate predicate, string pieceIdentifier, Tuple<int, int> relativeTo)
    {
        _actions = actions;
        _predicate = predicate;
        _pieceIdentifier = pieceIdentifier;
        _relativeTo = relativeTo;
    }

    /// <summary>
    /// Constructs a SpecialMove for each piece with the given _pieceIdentifier.
    /// The SpecialMove is constructed from the internal _actions, _relativeTo and that pieces position.
    /// 
    /// <para> Loops through all pieces of the given _pieceIdentifier.</para>
    /// <para>For each piece, calculate a move coordinate with a from and to coordinate.</para>
    /// <para>From is retrieved from the current piece position.</para>
    /// <para>To is the internal _relativeTo position relative to the current piece position´.</para>
    /// <para>Then construct a SpecialMove object with the internal _actions list and the calculated move coordinate.</para>
    /// <para>Perform the SpecialMove on a copied board</para>
    /// <para>Evaluate the internal _predicate, the supplied moveRule and check if the SpecialMove was performed successfully</para>
    /// <para>If the above is true, add it to the list of SpecialMove</para>
    /// </summary>
    /// <param name="thisBoard">The current board state used to construct the SpecialMove list from</param>
    /// <param name="moveRule">The moveRule of the RuleSet retrieving the valid moves</param>
    /// 
    /// <returns>A list of special moves that can be performed on the given board state.</returns>
    /// 
    public IEnumerable<SpecialMove> GetValidMoves(MoveWorker thisBoard, IPredicate moveRule)
    {
        List<string> positions = (List<string>) Utils.FindPiecesOfType(thisBoard, _pieceIdentifier);
        HashSet<SpecialMove> moves = new HashSet<SpecialMove>();

        foreach(string from in positions)
        {
            Tuple<int, int>? fromPos = thisBoard.Board.ParseCoordinate(from);
            if (fromPos == null) continue;
            Tuple<int, int> toPos = Tuple.Create(fromPos.Item1 + _relativeTo.Item1, fromPos.Item2 + _relativeTo.Item2);
            string? to;
            thisBoard.Board.IndexToCoor.TryGetValue(toPos, out to);
            if (to == null) continue;

            SpecialMove move = new SpecialMove(_actions, from + to);
            MoveWorker futureBoard = thisBoard.CopyBoardState();
            GameEvent result = move.Perform(futureBoard);
            
            BoardTransition transition = new BoardTransition(thisBoard, futureBoard, fromPos, toPos);
            if(_predicate.Evaluate(transition) && moveRule.Evaluate(transition) && result != GameEvent.InvalidMove)
            {
                moves.Add(move);
            }

        }

        return moves;
    }

    public static MoveData CastleMove(Player player, bool kingSide)
    {
        string KingIdentifier = player == Player.White ? Constants.WhiteKingIdentifier : Constants.BlackKingIdentifier;
        Player attacker = player == Player.White ? Player.Black : Player.White;
        
        IPredicate castlePredicate;

        string rank = player == Player.White ? "1" : "8";
        string kingFromFile = "e";
        string kingToFile = kingSide ? "g" : "c";
        string rookFromFile = kingSide ? "h" : "a";
        string rookToFile = kingSide ? "f" : "d";
        int kingSideMultiplier = kingSide ? 1 : -1;

        int relativeRookX = kingSide ? 3 : -4;

        Tuple<int, int> relativeRookPosition = Tuple.Create(0, relativeRookX);

        IPredicate kingCheckedThisState = new Attacked(BoardState.THIS, KingIdentifier);
        IPredicate kingCheckedNextState = new Attacked(BoardState.NEXT, KingIdentifier);

        IPredicate squareEmpty1 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(0, 1 * kingSideMultiplier), BoardState.THIS, PositionType.RELATIVE);
        IPredicate squareEmpty2 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(0, 2 * kingSideMultiplier), BoardState.THIS, PositionType.RELATIVE);
        IPredicate squareEmpty3 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(0, 3 * kingSideMultiplier), BoardState.THIS, PositionType.RELATIVE);

        IPredicate squareAttacked1 = new SquareAttacked(Tuple.Create(0, 1 * kingSideMultiplier), BoardState.THIS, attacker, PositionType.RELATIVE);
        IPredicate squareAttacked2 = new SquareAttacked(Tuple.Create(0, 2 * kingSideMultiplier), BoardState.THIS, attacker, PositionType.RELATIVE);

        IPredicate kingMoved = new HasMoved(Tuple.Create(0, 0), PositionType.RELATIVE);
        IPredicate rookMoved = new HasMoved(relativeRookPosition, PositionType.RELATIVE);

        castlePredicate = !kingCheckedThisState & !kingCheckedNextState & squareEmpty1 & squareEmpty2 & !squareAttacked1 & !squareAttacked2 & !kingMoved & !rookMoved;

        if (!kingSide)
            castlePredicate &= squareEmpty3;

        IEnumerable<IAction> castleActions = new List<IAction>
        {
            new ActionMovePieceAbsolute(kingFromFile + rank + kingToFile + rank),
            new ActionMovePieceAbsolute(rookFromFile + rank + rookToFile + rank)
        };

        return new MoveData(castleActions, castlePredicate, KingIdentifier, relativeRookPosition);
    }

    
    
    public static MoveData PawnDoubleMove(Player player)
    {
        int playerMultiplier = player == Player.White ? -1 : 1;
        string PawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        Tuple<int, int> forwardPosition1 = Tuple.Create(1 * playerMultiplier, 0);
        Tuple<int, int> forwardPosition2 = Tuple.Create(2 * playerMultiplier, 0);

        IPredicate hasMoved = new Const(false);// new HasMoved(Tuple.Create(0, 0), PositionType.RELATIVE);
        IPredicate targetSquareEmpty1 = new PieceAt(Constants.UnoccupiedSquareIdentifier, forwardPosition1, BoardState.THIS, PositionType.RELATIVE);
        IPredicate targetSquareEmpty2 = new PieceAt(Constants.UnoccupiedSquareIdentifier, forwardPosition2, BoardState.THIS, PositionType.RELATIVE);

        IEnumerable<IAction> actions = new List<IAction>
        {
            new ActionMovePieceRelative(forwardPosition2)
        };

        return new MoveData(actions, !hasMoved & targetSquareEmpty1 & targetSquareEmpty2, PawnIdentifier, forwardPosition2);
    }



    public static MoveData EnPassantMove(Player player, bool right)
    {
        int playerMultiplier = player == Player.White ? -1 : 1;
        int sideMultiplier = right ? 1 : -1;
        string PawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        string OpponentPawnIdentifier = player == Player.White ? Constants.BlackPawnIdentifier : Constants.WhitePawnIdentifier;

        Tuple<int, int> finalPositionRelative = Tuple.Create(1 * playerMultiplier, 1 * sideMultiplier);
        Tuple<int, int> enemyPawnPositionRelative = Tuple.Create(0, 1 * sideMultiplier);

        IPredicate enemyPawnNextTo = new PieceAt(OpponentPawnIdentifier, enemyPawnPositionRelative, BoardState.THIS, PositionType.RELATIVE);
        IPredicate targetSquareEmpty = new PieceAt(Constants.UnoccupiedSquareIdentifier, finalPositionRelative, BoardState.THIS, PositionType.RELATIVE);
        //IPredicate enemyPawnHasMoved = new HasMoved(enemyPawnPositionRelative, PositionType.RELATIVE);

        IEnumerable<IAction> actions = new List<IAction>
        {
            new ActionMovePieceRelative(finalPositionRelative),
            new ActionDeletePieceRelative(enemyPawnPositionRelative)
        };

        return new MoveData(actions, enemyPawnNextTo & targetSquareEmpty/* & enemyPawnHasMoved*/, PawnIdentifier, finalPositionRelative);
    }

}