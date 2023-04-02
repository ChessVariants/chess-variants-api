namespace ChessVariantsLogic;

using ChessVariantsLogic.Rules;
using ChessVariantsLogic.Rules.Moves;

/// <summary>
/// Retrieves and performs valid moves on a given Chessboard.
/// </summary>
public class MoveWorker
{

#region Fields, properties and constructors
    private Chessboard board;

    private readonly HashSet<Piece> pieces;

    public Chessboard Board
    {
        get { return this.board; }
        set { this.board = value; }
    }

    private readonly Dictionary<string, Piece> stringToPiece;

    private readonly List<Move> _moveLog;

    public List<Move> Movelog
    {
        get { return _moveLog; }
    }
    
    /// <summary>
    /// Constructor that takes a Chessboard and a HashSet of Piece
    /// </summary>
    /// <param name="chessboard">is the board that this worker should be assigned.</param>
    /// <param name="pieces">is the set of pieces that are used in the played variant.</param>
    public MoveWorker(Chessboard chessboard, HashSet<Piece> pieces, List<Move> moveLog)
    {
        this.board = chessboard;
        this.pieces = pieces;
        stringToPiece = initStringToPiece();
        _moveLog = moveLog;
    }
    public MoveWorker(Chessboard chessboard, HashSet<Piece> pieces) : this(chessboard, pieces, new List<Move>())
    {
    }

    public MoveWorker(Chessboard chessboard) : this(chessboard, new HashSet<Piece>()) {}


    #endregion

    /// <summary>
    /// Updates the chessboard by forcefully moving the square from the first coordinate to the last coordinate in move. The first coordinate will be marked as unoccupied.
    /// </summary>
    /// <param name="move"> consists of two coordinates without any space between them. </param>
    /// <param name="force"> If this is true it will not take into account whether or not this is a valid move for the piece. </param>
    /// <returns> A GameEvent representing whether the move was successful or not. </returns>
    public GameEvent Move(string move, bool force = false)
    {
        var splitMove = ParseMove(move);
        if (splitMove == null)
            return GameEvent.InvalidMove;

        string from = splitMove.Item1;
        string to = splitMove.Item2;

        string? strPiece = board.GetPieceIdentifier(from);
        if (strPiece == null) return GameEvent.InvalidMove;
        
        try
        {
            Piece piece = stringToPiece[strPiece];
            var moves = getAllValidMovesByPiece(piece, board.CoorToIndex[from]);
            var coor = board.ParseCoordinate(to);
            if (coor == null) return GameEvent.InvalidMove;

            if (force || moves.Contains(coor))
            {
                board.Insert(strPiece, to);
                board.Insert(Constants.UnoccupiedSquareIdentifier, from);
                board.PieceHasMoved(coor.Item1,coor.Item2);
                return GameEvent.MoveSucceeded;
            }
        }
        catch (KeyNotFoundException) { }
        
        return GameEvent.InvalidMove;
    }

    /// <summary>
    /// Adds the given move to the internal movelog. Should not be called outside of this class.
    /// </summary>
    private void AddMove(Move move)
    {
        _moveLog.Add(move);
    }

    /// <summary>
    /// Splits <paramref name="move"/> into the two corresponding substrings "from" and "to" squares.   
    /// </summary>
    /// <param name="move"> is a string representing two coordinates on the chessboard.</param>
    /// <returns> the two squares split into separate strings. </returns>
    public static Tuple<string, string>? ParseMove(string move)
    {
        string from = "", to = "";
        switch (move.Length)
        {
            case 4 : from = move.Substring(0,2); to = move.Substring(2,2); break;
            case 5 :
            {
                if(char.IsNumber(move[2]))
                {
                    from = move.Substring(0,3);
                    to = move.Substring(3,2);
                }
                else
                {
                    from = move.Substring(0,2);
                    to = move.Substring(2,3);
                }
                break;
            }
            case 6 : from = move.Substring(0,3); to = move.Substring(3,3); break;
            default: return null;
        }
        return new Tuple<string, string>(from, to);
    }
    /// <summary>
    /// Performs a move and adds it to the move log, this is a replacement for the old Move.Perform() method.
    /// </summary>
    /// <param name="move">The move to be performed.</param>
    /// <returns>A set of GameEvents that occured when the move was performed.</returns>
    public ISet<GameEvent> PerformMove(Move move)
    {
        AddMove(new Move(new List<Action>(), move.FromTo, move.PieceClassifier));
        return PerformActions(move.GetActions());
    }
    /// <summary>
    /// Performs a list of actions on this MoveWorker and adds the actions to the last move that was performed.
    /// </summary>
    /// <param name="actions">The set of actions to be performed and added.</param>
    /// <returns>A set of GameEvents that occured when the actions were performed.</returns>
    public ISet<GameEvent> PerformActions(List<Action> actions)
    {
        var events = new HashSet<GameEvent>();

        foreach (var action in actions)
        {
            events.Add(PerformAction(action));
        }
        return events;
    }

    /// <summary>
    /// Performs an action on this MoveWorker and adds the action to the last move that was performed.
    /// </summary>
    /// <param name="action">The action to be performed and added.</param>
    /// <returns>A GameEvent that occured when the action was performed.</returns>
    public GameEvent PerformAction(Action action)
    {
        var lastMove = GetLastMove();
        if (lastMove == null) throw new NullReferenceException("Can't add action if movelog is empty");
        var result = action.Perform(this, lastMove.From, lastMove.To);
        lastMove.AddAction(action);
        return result;
    }

    /// <summary>
    /// Runs the event on the given <paramref name="moveWorker"/>. This does not take into account whether the event actually should be run.
    /// </summary>
    /// <param name="moveWorker">The MoveWorker we want to run the event on.</param>
    /// <returns>A GameEvent that represents whether or not the event was successfully run./returns>
    public ISet<GameEvent> RunEvent(Event e)
    {
        var results = PerformActions(e.Actions);
        // Invalid events should just be ignored and not even be reported to the frontend.
        results.Remove(GameEvent.InvalidMove);
        return results;
    }

    /// <summary>
    /// Inserts given piece onto a square of the board
    /// </summary>
    /// <param name="piece">The piece to be inserted</param>
    /// <param name="square">The square that the piece should occupy.</param>
    /// <returns>True if the insertion was successful, otherwise false.</returns>
    public bool InsertOnBoard(Piece piece, string square)
    {
        if(Board.Insert(piece.PieceIdentifier, square))
        {
            if(!this.stringToPiece.ContainsKey(piece.PieceIdentifier))
                this.stringToPiece.Add(piece.PieceIdentifier, piece);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Gets all valid move for a given player.
    /// </summary>
    /// <param name="player"> is the player whose moves should be calculated. </param>
    /// <returns>an iterable collection of all valid moves.</returns>
    public HashSet<string> GetAllValidMoves(Player player)
    {
        var coorMoves = new HashSet<(Tuple<int,int>, Tuple<int,int>)>();

        foreach (var coor in this.board.GetAllCoordinates())
        {
            int row = coor.Item1;
            int col = coor.Item2;
            var square = this.board.GetPieceIdentifier(row, col);

            if(square == null || square.Equals(Constants.UnoccupiedSquareIdentifier))
                continue;

            Piece? p = null;
            try
            {
                p = this.stringToPiece[square];
            }
            catch (KeyNotFoundException)
            {
                continue;
            }

            if(pieceBelongsToPlayer(p, player))
            {
                var startPosition = new Tuple<int,int>(row, col);
                var legalMoves = getAllValidMovesByPiece(p, startPosition);
                foreach (var pos in legalMoves)
                {
                    coorMoves.Add((startPosition, pos));
                }
            }
        }
        return coorSetToStringSet(coorMoves);
    }


    /// <summary>
    /// <returns>The classifier of the given pieceIdentifier</returns>
    /// </summary>

    public PieceClassifier GetPieceClassifier(string pieceIdentifier)
    {
        Piece p = stringToPiece[pieceIdentifier];
        return p.PieceClassifier;
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">In case the move couldn't be parsed</exception>
    public Dictionary<string, List<string>> GetMoveDict(Player player)
    {
        var moveDict = new Dictionary<string, List<string>>();
        var moves = GetAllValidMoves(player);
        foreach (var move in moves)
        {
            var fromTo = ParseMove(move);
            if (fromTo == null)
            {
                throw new InvalidOperationException($"Could not parse move {move}");
            }

            var moveList = moveDict.GetValueOrDefault(fromTo.Item1, new List<string>());
            if (moveList.Count == 0)
            {
                moveDict[fromTo.Item1] = moveList;
            }
            moveList.Add(fromTo.Item2);

        }
        return moveDict;
    }

#region Move methods

    // Generates all moves for a piece.
    private HashSet<Tuple<int, int>> getAllValidMovesByPiece(Piece piece, Tuple<int,int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        foreach (var pattern in piece.GetAllMovementPatterns())
        {
            if(pattern is RegularPattern)
                moves.UnionWith(getRegularMoves(piece, pattern, pos));
            else
                moves.UnionWith(getJumpMove(piece, pattern, pos));
        }

        foreach (var pattern in piece.GetAllCapturePatterns())
        {
            if(pattern is RegularPattern)
            {
                moves.UnionWith(getRegularCaptureMoves(piece, pattern, pos));
            }
            else
            {
                var captureMove = getJumpCaptureMove(piece, pattern, pos);
                if(captureMove == null)
                    continue;
                moves.Add(captureMove);
            }
        }
        var movesTmp = moves.ToHashSet();
        int repeat = piece.Repeat;
        
        while (repeat >= 1)
        {
            foreach (var move in movesTmp)
            {
                foreach (var pattern in piece.GetAllMovementPatterns())
                {
                    if (pattern is RegularPattern)
                        moves.UnionWith(getRegularMoves(piece, pattern, move));
                    else
                        moves.UnionWith(getJumpMove(piece, pattern, move));
                }

                foreach (var pattern in piece.GetAllCapturePatterns())
                {
                    if (pattern is RegularPattern)
                    {
                        moves.UnionWith(getRegularCaptureMoves(piece, pattern, move));
                    }
                    else
                    {
                        var captureMove = getJumpCaptureMove(piece, pattern, move);
                        if (captureMove == null)
                            continue;
                        moves.Add(captureMove);
                    }
                }
            }
            movesTmp = moves;
            repeat--;
        }
        return moves;
    }

     // Returns all regular moves for a regularpattern.
    private HashSet<Tuple<int, int>> getRegularMoves(Piece piece, Pattern pattern, Tuple<int,int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        int maxIndex = Math.Max(board.Rows,board.Cols);

        if(pattern != null)
        
        
            for (int j = pattern.MinLength; j < maxIndex; j++)
            {
                int newRow = pos.Item1 + pattern.XDir * j;
                int newCol = pos.Item2 + pattern.YDir * j;

                if(!insideBoard(newRow, newCol))
                    break;

                string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
                string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

                if(pieceIdentifier1 == null || pieceIdentifier2 == null || hasTaken(piece, pos))
                    break;

                var minLength = pattern.MinLength;
                var maxLength = pattern.MaxLength;

                if(maxLength < j || j < minLength)
                    break;

                if(pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
                {
                    moves.Add(new Tuple<int, int>(newRow, newCol));
                    continue;
                } 
                break;
        }
        return moves;    
    }

    // Returns all valid capture moves for a regularpattern.
    private HashSet<Tuple<int, int>> getRegularCaptureMoves(Piece piece, Pattern pattern, Tuple<int, int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        int maxIndex = Math.Max(board.Rows,board.Cols);


        for (int j = 1; j < maxIndex; j++)
        {
    
            if (pattern == null)
                continue;

            int newRow = pos.Item1 + pattern.XDir * j;
            int newCol = pos.Item2 + pattern.YDir * j;

            if (!insideBoard(newRow, newCol))
                break;

            string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
            string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

            if (pieceIdentifier1 == null || pieceIdentifier2 == null || hasTaken(piece, pos))
                continue;

            var minLength = pattern.MinLength;
            var maxLength = pattern.MaxLength;

            if (maxLength < j || j < minLength)
                continue;

            if (pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
            {
                //moves.Add(new Tuple<int, int>(newRow, newCol));
                continue;
            }

            Piece? piece2 = null;

            try
            {
                piece2 = this.stringToPiece[pieceIdentifier2];
            }
            catch (KeyNotFoundException)
            {
                continue;
            }

            if (piece.CanTake(piece2))
                moves.Add(new Tuple<int, int>(newRow, newCol));

            break;

            }
        
        return moves;    
    }

    // Returns move for jumpingpattern
    private HashSet<Tuple<int, int>> getJumpMove(Piece piece, Pattern pattern, Tuple<int, int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();

        int newRow = pos.Item1 + pattern.XDir;
        int newCol = pos.Item2 + pattern.YDir;

        string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
        string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

        if(pieceIdentifier1 == null || pieceIdentifier2 == null)
            return new HashSet<Tuple<int, int>>();

        if(pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
        {
            moves.Add(new Tuple<int, int>(newRow, newCol));
        }
        return moves;
    }

    // Returns capture move for a jumpingpattern.
    private Tuple<int,int>? getJumpCaptureMove(Piece piece, Pattern pattern, Tuple<int, int> pos)
    {
        
        if (pattern == null)
            return null;

        int newRow = pos.Item1 + pattern.XDir;
        int newCol = pos.Item2 + pattern.YDir;

        string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
        string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

        if (pieceIdentifier1 == null || pieceIdentifier2 == null || pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
            return null;

        Piece? piece1 = null;
        Piece? piece2 = null;

        try
        {
            piece1 = this.stringToPiece[pieceIdentifier1];
            piece2 = this.stringToPiece[pieceIdentifier2];
        }
        catch (KeyNotFoundException)
        {
            return new Tuple<int, int>(-1,-1);
        }

        if (!insideBoard(newRow, newCol) || !piece1.CanTake(piece2))
            return null;
        
        return new Tuple<int,int>(newRow, newCol); 
    }

    /// <summary>
    /// Gets all valid capture moves to empty squares for a given player.
    /// </summary>
    /// <param name="player"> is the player whose moves should be calculated. </param>
    /// <returns>an iterable collection of all valid capture moves.</returns>
    public HashSet<string> GetAllCapturePatternMoves(Player player)
    {
        var coorMoves = new HashSet<(Tuple<int,int>, Tuple<int,int>)>();

        foreach (var coor in this.board.GetAllCoordinates())
        {
            int row = coor.Item1;
            int col = coor.Item2;
            var square = this.board.GetPieceIdentifier(row, col);

            if(square == null || square.Equals(Constants.UnoccupiedSquareIdentifier))
                continue;

            Piece? p = null;
            try
            {
                p = this.stringToPiece[square];
            }
            catch (KeyNotFoundException)
            {
                continue;
            }

            if(pieceBelongsToPlayer(p, player))
            {
                var startPosition = new Tuple<int,int>(row, col);
                var legalMoves = getAllValidCaptureMovesByPiece(p, startPosition);
                foreach (var pos in legalMoves)
                {
                    coorMoves.Add((startPosition, pos));
                }
            }
        }
        return coorSetToStringSet(coorMoves);
    }

    // Generates all capture moves for a piece.
    private HashSet<Tuple<int, int>> getAllValidCaptureMovesByPiece(Piece piece, Tuple<int, int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        var capturemoves = new HashSet<Tuple<int, int>>();

        int repeat = piece.Repeat;
        
        foreach (var pattern in piece.GetAllCapturePatterns())
        {
            if (pattern is RegularPattern)
            {
                capturemoves.UnionWith(getRegularMoves(piece, pattern, pos));
                capturemoves.UnionWith(getRegularCaptureMoves(piece, pattern, pos));
            }
            else
            {
                capturemoves.UnionWith(getJumpMove(piece, pattern, pos));
                var captureMove = getJumpCaptureMove(piece, pattern, pos);
                if(captureMove == null)
                    continue;
                capturemoves.Add(captureMove);
            }
        }

        foreach (var pattern in piece.GetAllMovementPatterns())
        {
            if (pattern is RegularPattern)
                moves.UnionWith(getRegularMoves(piece, pattern, pos));
            else
                moves.UnionWith(getJumpMove(piece, pattern, pos));
        }

        var movesTmp = moves.ToHashSet();

        while (repeat >= 1)
        {
            foreach (var move in movesTmp)
            {
                foreach (var pattern in piece.GetAllMovementPatterns())
                {
                    if (pattern is RegularPattern)
                        moves.UnionWith(getRegularMoves(piece, pattern, move));
                    else
                        moves.UnionWith(getJumpMove(piece, pattern, move));
                }
                foreach (var pattern in piece.GetAllCapturePatterns())
                {
                    if (pattern is RegularPattern)
                    {    
                        capturemoves.UnionWith(getRegularMoves(piece, pattern, move));
                        capturemoves.UnionWith(getRegularCaptureMoves(piece, pattern, pos));
                    }
                    else
                    {
                        capturemoves.UnionWith(getJumpMove(piece, pattern, move));
                        var captureMove = getJumpCaptureMove(piece, pattern, pos);
                        if(captureMove == null)
                            continue;
                        capturemoves.Add(captureMove);
                    }
                }
            }
            movesTmp = moves;
            repeat--;
        }
        return capturemoves;
    }

#endregion

#region Auxiliary methods

    // Converts a HashSet of coordinates with start and end coordinate into string representation
    private HashSet<String> coorSetToStringSet(HashSet<(Tuple<int,int>, Tuple<int,int>)> coorMoves)
    {
        var moves = new HashSet<string>();
        foreach (var move in coorMoves)
        {
            string start = this.board.IndexToCoor[move.Item1];
            string end = this.board.IndexToCoor[move.Item2];
            moves.Add(start + end);
        }
        return moves;
    }

    // PieceClassifier and Player should maybe be merged into one common enum.
    private bool pieceBelongsToPlayer(Piece piece, Player player)
    {
        return player.Equals(Player.White) && piece.PieceClassifier.Equals(PieceClassifier.WHITE)
            || player.Equals(Player.Black) && piece.PieceClassifier.Equals(PieceClassifier.BLACK)
            || piece.PieceClassifier.Equals(PieceClassifier.SHARED);
    }
    
    private bool insideBoard(int row, int col)
    {
        return 0 <= row && row < this.board.Rows && 0 <= col && col < this.board.Cols;
    }
    private Dictionary<string, Piece> initStringToPiece()
    {
        var dictionary = new Dictionary<string, Piece>();

        foreach (Piece p in this.pieces)
        {
            dictionary.Add(p.PieceIdentifier, p);   
        }

        return dictionary;
    }

    private bool hasTaken(Piece piece1, Tuple<int,int> pos)
    {
        string? piece2 = board.GetPieceIdentifier(pos);
        
        if(piece2 != null)
        {
            if(piece2.Equals(Constants.UnoccupiedSquareIdentifier))
                return false;
            Piece p2 = this.stringToPiece[piece2];
            return piece1.CanTake(p2);
        }
        return false;
    }

    /// <summary>
    /// Copies this move worker to a new move worker object
    /// </summary>
    public MoveWorker CopyBoardState()
    {
        Chessboard newBoard = board.CopyBoard();
        HashSet<Piece> newPieces = new HashSet<Piece>(pieces);
        List<Move> moveLog = new List<Move>(_moveLog);
        
        return new MoveWorker(newBoard, newPieces, moveLog);
    }

    /// <summary>
    /// Returns the last move from the movelog
    /// </summary>
    public Move? GetLastMove()
    {
        if (Movelog.Count == 0)
            return null;
        return Movelog.Last();
    }

    #endregion

}