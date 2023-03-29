namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Moves.Actions;
using ChessVariantsLogic.Rules.Predicates;
using ChessVariantsLogic.Rules.Predicates.ChessPredicates;

/// <summary>
/// Factory with predetermined rules for the known variants. WIP
/// </summary>
public static class GameFactory
{

    private static OperatorType AND = OperatorType.AND;
    private static OperatorType OR = OperatorType.OR;
    private static OperatorType IMPLIES = OperatorType.IMPLIES;
    private static OperatorType XOR = OperatorType.XOR;
    private static OperatorType EQUALS = OperatorType.EQUALS;
    private static OperatorType NOT = OperatorType.NOT;

    public const string StandardIdentifier = "standard";
    public const string CaptureTheKingIdentifier = "captureTheKing";
    public const string AntiChessIdentifier = "antiChess";
    public const string DuckChessIdentifier = "duckChess";
    public const string AtomicChessIdentifier = "atomicChess";

    public static Game StandardChess()
    {
        IPredicate blackKingCheckedNextTurn = new Attacked(BoardState.NEXT, Constants.BlackKingIdentifier);
        IPredicate whiteKingCheckedNextTurn = new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier);

        IPredicate whiteMoveRule = !whiteKingCheckedNextTurn;
        IPredicate blackMoveRule = !blackKingCheckedNextTurn;



        ISet<MoveTemplate> movesWhite = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.White, true, false),
            MoveTemplate.CastleMove(Player.White, false, false),
            MoveTemplate.PawnDoubleMove(Player.White),
            MoveTemplate.EnPassantMove(Player.White, true),
            MoveTemplate.EnPassantMove(Player.White, false),
        };


        ISet<MoveTemplate> movesBlack = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.Black, true, false),
            MoveTemplate.CastleMove(Player.Black, false, false),
            MoveTemplate.PawnDoubleMove(Player.Black),
            MoveTemplate.EnPassantMove(Player.Black, true),
            MoveTemplate.EnPassantMove(Player.Black, false),
        };

        Event whiteWin = Event.WinEvent(Player.White, blackKingCheckedNextTurn);
        Event blackWin = Event.WinEvent(Player.Black, whiteKingCheckedNextTurn);

        Event whiteTie = Event.TieEvent(!blackKingCheckedNextTurn);
        Event blackTie = Event.TieEvent(!whiteKingCheckedNextTurn);

        ISet<Event> eventsWhite = new HashSet<Event> { Event.PromotionEvent(Player.White, 8) };
        ISet<Event> eventsBlack = new HashSet<Event> { Event.PromotionEvent(Player.Black, 8) };

        ISet<Event> noMovesLeftEventsWhite = new HashSet<Event> { blackWin, blackTie };
        ISet<Event> noMovesLeftEventsBlack = new HashSet<Event> { whiteWin, whiteTie };

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, movesWhite, eventsWhite, noMovesLeftEventsWhite);
        RuleSet rulesBlack = new RuleSet(blackMoveRule, movesBlack, eventsBlack, noMovesLeftEventsBlack);


        return new Game(new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces()), Player.White, 1, rulesWhite, rulesBlack);
    }


    public static Game CaptureTheKing()
    {
        IPredicate whiteMoveRule = new Const(true);
        IPredicate whiteWinRule = new PiecesLeft(Constants.BlackKingIdentifier, Comparator.EQUALS, 0, BoardState.NEXT);

        
        IPredicate blackMoveRule = new Const(true);
        IPredicate blackWinRule = new PiecesLeft(Constants.WhiteKingIdentifier, Comparator.EQUALS, 0, BoardState.NEXT);

        ISet<MoveTemplate> movesWhite = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.White, true, true),
            MoveTemplate.CastleMove(Player.White, false, true),
            MoveTemplate.PawnDoubleMove(Player.White),
            MoveTemplate.EnPassantMove(Player.White, true),
            MoveTemplate.EnPassantMove(Player.White, false),
        };


        ISet<MoveTemplate> movesBlack = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.Black, true, true),
            MoveTemplate.CastleMove(Player.Black, false, true),
            MoveTemplate.PawnDoubleMove(Player.Black),
            MoveTemplate.EnPassantMove(Player.Black, true),
            MoveTemplate.EnPassantMove(Player.Black, false),
        };

        ISet<Event> eventsWhite = new HashSet<Event> { Event.PromotionEvent(Player.White, 8), Event.WinEvent(Player.White, whiteWinRule) };
        ISet<Event> eventsBlack = new HashSet<Event> { Event.PromotionEvent(Player.Black, 8), Event.WinEvent(Player.Black, blackWinRule) };


        RuleSet rulesWhite = new RuleSet(whiteMoveRule, movesWhite, eventsWhite, new HashSet<Event>() { Event.TieEvent(new Const(true)) });
        RuleSet rulesBlack = new RuleSet(blackMoveRule, movesBlack, eventsBlack, new HashSet<Event>() { Event.TieEvent(new Const(true)) });

        return new Game(new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces()), Player.White, 1, rulesWhite, rulesBlack);

    }


    public static Game DuckChess()
    {

        IPredicate lastMoveWasDuck = new LastMoveClassifier(PieceClassifier.SHARED);
        IPredicate lastMoveWasBlack = new LastMoveClassifier(PieceClassifier.BLACK);
        IPredicate lastMoveWasWhite = new LastMoveClassifier(PieceClassifier.WHITE);

        IPredicate thisMoveWasDuckMove = new PieceMoved(Constants.DuckIdentifier);

        IPredicate firstMove = new FirstMove();

        IPredicate whiteMoveRule = ((thisMoveWasDuckMove & lastMoveWasWhite) | (!thisMoveWasDuckMove & (lastMoveWasDuck | firstMove)));
        IPredicate whiteWinRule = new PiecesLeft(Constants.BlackKingIdentifier, Comparator.EQUALS, 0, BoardState.THIS);

        IPredicate blackMoveRule = ((thisMoveWasDuckMove & lastMoveWasBlack) | (!thisMoveWasDuckMove & (lastMoveWasDuck | firstMove)));
        IPredicate blackWinRule = new PiecesLeft(Constants.WhiteKingIdentifier, Comparator.EQUALS, 0, BoardState.THIS);

        ISet<MoveTemplate> movesWhite = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.White, true, true),
            MoveTemplate.CastleMove(Player.White, false, true),
            MoveTemplate.PawnDoubleMove(Player.White),
            MoveTemplate.EnPassantMove(Player.White, true),
            MoveTemplate.EnPassantMove(Player.White, false),
        };

        ISet<MoveTemplate> movesBlack = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.Black, true, true),
            MoveTemplate.CastleMove(Player.Black, false, true),
            MoveTemplate.PawnDoubleMove(Player.Black),
            MoveTemplate.EnPassantMove(Player.Black, true),
            MoveTemplate.EnPassantMove(Player.Black, false),
        };

        ISet<Event> eventsWhite = new HashSet<Event> { Event.PromotionEvent(Player.White, 8), Event.WinEvent(Player.White, whiteWinRule) };
        ISet<Event> eventsBlack = new HashSet<Event> { Event.PromotionEvent(Player.Black, 8), Event.WinEvent(Player.Black, blackWinRule) };

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, movesWhite, eventsWhite, new HashSet<Event>() { Event.TieEvent(new Const(true)) });
        RuleSet rulesBlack = new RuleSet(blackMoveRule, movesBlack, eventsBlack, new HashSet<Event>() { Event.TieEvent(new Const(true)) });

        return new Game(new MoveWorker(Chessboard.DuckChessboard(), Piece.AllDuckChessPieces()), Player.White, 2, rulesWhite, rulesBlack);

    }

    // This is just for fun and to show that the event system is general.
    // More info: https://en.wikipedia.org/wiki/Atomic_chess

    public static Game AtomicChess()
    {
        IPredicate blackKingCheckedNextTurn = new Attacked(BoardState.NEXT, Constants.BlackKingIdentifier);
        IPredicate whiteKingCheckedNextTurn = new Attacked(BoardState.NEXT, Constants.WhiteKingIdentifier);

        IPredicate whiteKingLeftNextTurn = new PiecesLeft(Constants.WhiteKingIdentifier, Comparator.GREATER_THAN, 0, BoardState.NEXT);
        IPredicate blackKingLeftNextTurn = new PiecesLeft(Constants.BlackKingIdentifier, Comparator.GREATER_THAN, 0, BoardState.NEXT);

        IPredicate whiteMoveRule = !whiteKingCheckedNextTurn & whiteKingLeftNextTurn;
        IPredicate blackMoveRule = !blackKingCheckedNextTurn & blackKingLeftNextTurn;


        ISet<MoveTemplate> movesWhite = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.White, true, false),
            MoveTemplate.CastleMove(Player.White, false, false),
            MoveTemplate.PawnDoubleMove(Player.White),
            MoveTemplate.EnPassantMove(Player.White, true),
            MoveTemplate.EnPassantMove(Player.White, false),
        };


        ISet<MoveTemplate> movesBlack = new HashSet<MoveTemplate>
        {
            MoveTemplate.CastleMove(Player.Black, true, false),
            MoveTemplate.CastleMove(Player.Black, false, false),
            MoveTemplate.PawnDoubleMove(Player.Black),
            MoveTemplate.EnPassantMove(Player.Black, true),
            MoveTemplate.EnPassantMove(Player.Black, false),
        };

        Event whiteWin = Event.WinEvent(Player.White, blackKingCheckedNextTurn);
        Event blackWin = Event.WinEvent(Player.Black, whiteKingCheckedNextTurn);

        Event whiteTie = Event.TieEvent(!blackKingCheckedNextTurn);
        Event blackTie = Event.TieEvent(!whiteKingCheckedNextTurn);

        ISet<Event> eventsWhite = new HashSet<Event> { Event.PromotionEvent(Player.White, 8) };
        ISet<Event> eventsBlack = new HashSet<Event> { Event.PromotionEvent(Player.Black, 8) };

        ISet<Event> stalemateEventsWhite = new HashSet<Event> { blackWin, blackTie };
        ISet<Event> stalemateEventsBlack = new HashSet<Event> { whiteWin, whiteTie };

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                IPosition position = new PositionRelative(y, x);
                bool shouldDestroyPawn = (x == 0 && y == 0);
                eventsWhite.Add(Event.ExplosionEvent(Player.White, position, shouldDestroyPawn));
                eventsBlack.Add(Event.ExplosionEvent(Player.Black, position, shouldDestroyPawn));
            }
        }


        RuleSet rulesWhite = new RuleSet(whiteMoveRule, movesWhite, eventsWhite, stalemateEventsWhite);
        RuleSet rulesBlack = new RuleSet(blackMoveRule, movesBlack, eventsBlack, stalemateEventsBlack);


        return new Game(new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces()), Player.White, 1, rulesWhite, rulesBlack);
    }
    public static Game AntiChess()
    {
        IPredicate whiteMoveRule = new Operator(new Attacked(BoardState.THIS, "ANY_BLACK"), IMPLIES, new PieceCaptured("ANY_BLACK"));
        IPredicate whiteWinRule = new PiecesLeft("ANY_WHITE", Comparator.EQUALS, 0, BoardState.NEXT);
        
        IPredicate blackMoveRule = new Operator(new Attacked(BoardState.THIS, "ANY_WHITE"), IMPLIES, new PieceCaptured("ANY_WHITE"));
        IPredicate blackWinRule = new PiecesLeft("ANY_BLACK", Comparator.EQUALS, 0, BoardState.NEXT);

        ISet<MoveTemplate> movesWhite = new HashSet<MoveTemplate>
        {
            MoveTemplate.PawnDoubleMove(Player.White),
            MoveTemplate.EnPassantMove(Player.White, true),
            MoveTemplate.EnPassantMove(Player.White, false),
        };


        ISet<MoveTemplate> movesBlack = new HashSet<MoveTemplate>
        {
            MoveTemplate.PawnDoubleMove(Player.Black),
            MoveTemplate.EnPassantMove(Player.Black, true),
            MoveTemplate.EnPassantMove(Player.Black, false),
        };

        ISet<Event> eventsWhite = new HashSet<Event> { Event.WinEvent(Player.White, whiteWinRule) };
        ISet<Event> eventsBlack = new HashSet<Event> { Event.WinEvent(Player.Black, blackWinRule) };

        ISet<Event> stalemateEventsWhite = new HashSet<Event> { Event.WinEvent(Player.White, new Const(true)) };
        ISet<Event> stalemateEventsBlack = new HashSet<Event> { Event.WinEvent(Player.Black, new Const(true)) };

        RuleSet rulesWhite = new RuleSet(whiteMoveRule, movesWhite, eventsWhite, stalemateEventsWhite);
        RuleSet rulesBlack = new RuleSet(blackMoveRule, movesBlack, eventsBlack, stalemateEventsBlack);

        return new Game(new MoveWorker(Chessboard.StandardChessboard(), Piece.AllStandardPieces()), Player.White, 1, rulesWhite, rulesBlack);
    }

    /// <summary>
    /// Returns a game corresponding to the <paramref name="identifier"/>>.
    /// </summary>
    /// <param name="identifier">Variant identifier for the variant to create</param>
    /// <returns>A game corresponding to the <paramref name="identifier"/>>.</returns>
    /// <exception cref="ArgumentException">If there's no correspondence to the identifier</exception>
    public static Game FromIdentifier(string identifier)
    {
        return identifier switch
        {
            StandardIdentifier => StandardChess(),
            AntiChessIdentifier => AntiChess(),
            CaptureTheKingIdentifier => CaptureTheKing(),
            DuckChessIdentifier => DuckChess(),
            AtomicChessIdentifier => AtomicChess(),
            _ => throw new ArgumentException($"No variant corresponds to identifier: {identifier}"),
        };

    }
}