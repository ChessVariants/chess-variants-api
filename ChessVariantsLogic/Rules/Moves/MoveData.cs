using ChessVariantsLogic.Rules.Moves.Actions;

namespace ChessVariantsLogic.Rules.Moves;

using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using Predicates;
using System;

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

    public IEnumerable<Move> GetValidMoves(IBoardState thisBoard, IPredicate moveRule)
    {
        List<string> positions = (List<string>) Utils.FindPiecesOfType(thisBoard, _pieceIdentifier);
        HashSet<Move> moves = new HashSet<Move>();

        foreach(string from in positions)
        {
            Tuple<int, int>? fromPos = thisBoard.Board.ParseCoordinate(from);
            if (fromPos == null) continue;
            Tuple<int, int> toPos = Tuple.Create(fromPos.Item1 + _relativeTo.Item1, fromPos.Item2 + _relativeTo.Item2);
            string? to;
            thisBoard.Board.IndexToCoor.TryGetValue(toPos, out to);
            if (to == null) continue;

            Move move = new Move(_actions, from + to);
            IBoardState futureBoard = thisBoard.CopyBoardState();
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

        Tuple<int, int> relativeRookPosition = Tuple.Create(relativeRookX, 0);

        IPredicate kingCheckedThisState = new Attacked(BoardState.THIS, KingIdentifier);
        IPredicate kingCheckedNextState = new Attacked(BoardState.NEXT, KingIdentifier);

        IPredicate squareEmpty1 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(1 * kingSideMultiplier, 0), BoardState.THIS, PositionType.RELATIVE);
        IPredicate squareEmpty2 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(2 * kingSideMultiplier, 0), BoardState.THIS, PositionType.RELATIVE);
        IPredicate squareEmpty3 = new PieceAt(Constants.UnoccupiedSquareIdentifier, Tuple.Create(3 * kingSideMultiplier, 0), BoardState.THIS, PositionType.RELATIVE);

        IPredicate squareAttacked1 = new SquareAttacked(Tuple.Create(1 * kingSideMultiplier, 0), BoardState.THIS, attacker, PositionType.RELATIVE);
        IPredicate squareAttacked2 = new SquareAttacked(Tuple.Create(2 * kingSideMultiplier, 0), BoardState.THIS, attacker, PositionType.RELATIVE);

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
        Tuple<int, int> forwardPosition = Tuple.Create(0, 2 * playerMultiplier);

        IPredicate hasMoved = new HasMoved(Tuple.Create(0, 0), PositionType.RELATIVE);

        IEnumerable<IAction> actions = new List<IAction>
        {
            new ActionMovePieceRelative(forwardPosition)
        };

        return new MoveData(actions, !hasMoved, PawnIdentifier, forwardPosition);
    }

    public static MoveData EnPassantMove(Player player, bool right)
    {
        int playerMultiplier = player == Player.White ? -1 : 1;
        int sideMultiplier = right ? 1 : -1;
        string PawnIdentifier = player == Player.White ? Constants.WhitePawnIdentifier : Constants.BlackPawnIdentifier;
        string OpponentPawnIdentifier = player == Player.White ? Constants.BlackPawnIdentifier : Constants.WhitePawnIdentifier;

        Tuple<int, int> finalPositionRelative = Tuple.Create(1 * sideMultiplier, 1 * playerMultiplier);
        Tuple<int, int> enemyPawnPositionRelative = Tuple.Create(1 * sideMultiplier, 0);

        IPredicate enemyPawnNextTo = new PieceAt(OpponentPawnIdentifier, enemyPawnPositionRelative, BoardState.THIS, PositionType.RELATIVE);
        IPredicate enemyPawnHasMoved = new HasMoved(enemyPawnPositionRelative, PositionType.RELATIVE);

        IEnumerable<IAction> actions = new List<IAction>
        {
            new ActionMovePieceRelative(finalPositionRelative),
            new ActionDeletePieceRelative(enemyPawnPositionRelative)
        };

        return new MoveData(actions, enemyPawnNextTo & enemyPawnHasMoved, PawnIdentifier, finalPositionRelative);
    }

}