namespace ChessVariantsLogic;
using static Piece;
using System;

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
    
    /// <summary>
    /// Constructor that takes a Chessboard and a HashSet of Piece
    /// </summary>
    /// <param name="chessboard">is the board that this worker should be assigned.</param>
    /// <param name="pieces">is the set of pieces that are used in the played variant.</param>
    public MoveWorker(Chessboard chessboard, HashSet<Piece> pieces)
    {
        this.board = chessboard;
        this.pieces = pieces;
        stringToPiece = initStringToPiece();
    }

    public MoveWorker(Chessboard chessboard) : this(chessboard, new HashSet<Piece>()) {}

#endregion

    /// <summary>
    /// Updates the chessboard by moving the square from the first coordinate to the last coordinate in move. The first coordinate will be marked as unoccupied.
    /// </summary>
    /// <param name="move"> consists of two coordinates without any space between them. </param>
    /// <returns> A GameEvent representing whether the move was successful or not. </returns>
    public GameEvent Move(string move)
    {
        var splitMove = parseMove(move);
        if(splitMove == null)
            return GameEvent.InvalidMove;

        string from = splitMove.Item1;
        string to = splitMove.Item2;
        
        string? strPiece = this.board.GetPieceIdentifier(from);
        if(strPiece != null)
        {
            try
            {
                Piece piece = stringToPiece[strPiece];
                var moves = getAllValidMovesByPiece(piece, this.board.CoorToIndex[from]);
                var coor = this.board.ParseCoordinate(to);
                if(coor != null && moves.Contains(coor))
                {
                    this.board.Insert(strPiece, to);
                    this.board.Insert(Constants.UnoccupiedSquareIdentifier, from);
                    return GameEvent.MoveSucceeded;
                }
            }
            catch (KeyNotFoundException) {}
        }
        return GameEvent.InvalidMove;

    }

    /// <summary>
    /// Splits <paramref name="move"/> into the two corresponding substrings "from" and "to" squares.   
    /// </summary>
    /// <param name="move"> is a string representing two coordinates on the chessboard.</param>
    /// <returns> the two squares split into separate strings. </returns>
    public Tuple<string, string>? parseMove(string move)
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

#region Move methods
// These methods are approximately sorted in the order that they are called.
//First comes all methods regarding RegularMovementPattern and then the corresponding methods for JumpMovementPattern.

    // Returns all valid moves for a given piece.
    private HashSet<Tuple<int, int>> getAllValidMovesByPiece(Piece piece, Tuple<int, int> pos)
    {
        if (piece.MovementPattern is JumpMovementPattern)
            return generateJumpMoves(piece, pos);

        return generateRegularMoves(piece, pos);
    }

    // Generates all moves for a piece that can not jump over other pieces.
    private HashSet<Tuple<int, int>> generateRegularMoves(Piece piece, Tuple<int,int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        var movesTmp = getAllMoves(piece, pos);
        int repeat = piece.Repeat;

        moves = getAllMoves(piece, pos);
        moves.UnionWith(getAllValidCapturesByPiece(piece, pos));
        
        while (repeat >= 1)
        {
            foreach (var move in movesTmp)
            {
                moves.UnionWith(getAllMoves(piece, new Tuple<int, int>(move.Item1, move.Item2)));
                moves.UnionWith(getAllCaptureMoves(piece, new Tuple<int, int>(move.Item1, move.Item2)));
            }
            movesTmp = moves;
            repeat--;
        }
        return moves;
    }

     // Returns all moves for a non-jumping piece.
    private HashSet<Tuple<int, int>> getAllMoves(Piece piece, Tuple<int, int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        int maxIndex = Math.Max(board.Rows,board.Cols);

        for (int i = 0; i < piece.GetMovementPatternCount(); i++)
        {
            for (int j = 1; j < maxIndex; j++)
            {
                var pattern = piece.GetMovementPattern(i);
                if(pattern == null)
                    continue;

                int newRow = pos.Item1 + pattern.Item1 * j;
                int newCol = pos.Item2 + pattern.Item2 * j;

                if(!insideBoard(newRow, newCol))
                    break;

                string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
                string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

                if(pieceIdentifier1 == null || pieceIdentifier2 == null || hasTaken(piece, pos))
                    continue;

                var moveLength = piece.GetMoveLength(pattern);
                if(moveLength == null)
                    continue;
                
                var ml1 = moveLength.Item1;
                var ml2 = moveLength.Item2;

                if(ml2 < j || j < ml1)
                    continue;

                if(pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
                {
                    moves.Add(new Tuple<int, int>(newRow, newCol));
                    continue;
                }
                break;

                /*Piece? piece2 = null;

                try
                {
                    piece2 = this.stringToPiece[pieceIdentifier2];
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }
                
                if (canTake(piece, piece2))
                    moves.Add(new Tuple<int, int>(newRow, newCol));
                    
                break; */

            }
        }
        return moves;    
    }

    // Returns all valid capture moves for a non-jumping piece.
    private HashSet<Tuple<int, int>> getAllCaptureMoves(Piece piece, Tuple<int, int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        int maxIndex = Math.Max(board.Rows,board.Cols);

        for (int i = 0; i < piece.GetCapturePatternCount(); i++)
        {
            for (int j = 1; j < maxIndex; j++)
            {
                var pattern = piece.GetCapturePattern(i);
                if(pattern == null)
                    continue;

                int newRow = pos.Item1 + pattern.Item1 * j;
                int newCol = pos.Item2 + pattern.Item2 * j;

                if(!insideBoard(newRow, newCol))
                    break;

                string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
                string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

                if(pieceIdentifier1 == null || pieceIdentifier2 == null || hasTaken(piece, pos))
                    continue;

                var moveLength = piece.GetCaptureLength(pattern);
                if(moveLength == null)
                    continue;
                
                var ml1 = moveLength.Item1;
                var ml2 = moveLength.Item2;

                if(ml2 < j || j < ml1)
                    continue;

                if(pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
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
                
                if (canTake(piece, piece2))
                    moves.Add(new Tuple<int, int>(newRow, newCol));
                    
                break;

            }
        }
        return moves;    
    }

    // Returns all moves for a jumping piece.
    private HashSet<Tuple<int, int>> getAllMovesJump(Piece piece, Tuple<int, int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        for (int i = 0; i < piece.GetMovementPatternCount(); i++)
        {

            var pattern = piece.GetMovementPattern(i);
            if(pattern == null)
                continue;

            int newRow = pos.Item1 + pattern.Item1;
            int newCol = pos.Item2 + pattern.Item2;

            string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
            string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

            if(pieceIdentifier1 == null || pieceIdentifier2 == null)
                continue;

            if(pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
            {
                moves.Add(new Tuple<int, int>(newRow, newCol));
                continue;
            }

           /* Piece? piece1 = null;
            Piece? piece2 = null;

            try
            {
                piece1 = this.stringToPiece[pieceIdentifier1];
                piece2 = this.stringToPiece[pieceIdentifier2];
            }
            catch (KeyNotFoundException)
            {
                continue;
            }

            if (insideBoard(newRow, newCol) && canTake(piece1, piece2))
                moves.Add(new Tuple<int, int>(newRow, newCol));*/

        }
        return moves;
    }

    // Generates all moves for a piece that can jump over other pieces.
    private HashSet<Tuple<int, int>> generateJumpMoves(Piece piece, Tuple<int,int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        var movesTmp = getAllMovesJump(piece, pos);
        int repeat = piece.Repeat;

        moves = getAllMovesJump(piece, pos);
        moves.UnionWith(getAllValidCapturesByPiece(piece, pos));
        
        while (repeat >= 1)
        {
            foreach (var move in movesTmp)
            {
                moves.UnionWith(getAllMovesJump(piece, new Tuple<int, int>(move.Item1, move.Item2)));
                moves.UnionWith(getAllCapturesJump(piece, new Tuple<int, int>(move.Item1, move.Item2)));
            }
            movesTmp = moves;
            repeat--;
        }
        return moves;
    }

    // Returns all valid capture moves for a given piece.
    private HashSet<Tuple<int, int>> getAllValidCapturesByPiece(Piece piece, Tuple<int, int> pos)
    {
        if (piece.CapturePattern is JumpMovementPattern)
            return getAllCapturesJump(piece, pos);

        return getAllCaptureMoves(piece, pos);
    }

    // Returns all valid moves for a piece that can jump.
    private HashSet<Tuple<int, int>> getAllCapturesJump(Piece piece, Tuple<int, int> pos)
    {
        var moves = new HashSet<Tuple<int, int>>();
        for (int i = 0; i < piece.GetCapturePatternCount(); i++)
        {

            var pattern = piece.GetCapturePattern(i);
            if(pattern == null)
                continue;

            int newRow = pos.Item1 + pattern.Item1;
            int newCol = pos.Item2 + pattern.Item2;

            string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
            string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

            if(pieceIdentifier1 == null || pieceIdentifier2 == null || pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
                continue;

            Piece? piece1 = null;
            Piece? piece2 = null;

            try
            {
                piece1 = this.stringToPiece[pieceIdentifier1];
                piece2 = this.stringToPiece[pieceIdentifier2];
            }
            catch (KeyNotFoundException)
            {
                continue;
            }

            if (insideBoard(newRow, newCol) && canTake(piece1, piece2))
                moves.Add(new Tuple<int, int>(newRow, newCol));

        }
        return moves;
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
            || player.Equals(Player.Black) && piece.PieceClassifier.Equals(PieceClassifier.BLACK);
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
            return canTake(piece1,p2);
        }
        return false;
    }

#endregion

}

public enum GameEvent {
    InvalidMove,
    MoveSucceeded,
    WhiteWon,
    BlackWon,
    Tie
}


public enum Player {
    White,
    Black
}