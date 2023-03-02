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
        var (from, to) = parseMove(move);
        
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

    // Splits the string move into the substrings representing the "from" square and "to" square 
    public (string, string) parseMove(string move)
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
        }
        return (from, to);
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
    public List<string> GetAllValidMoves(Player player)
    {
        var coorMoves = new List<(Tuple<int,int>, Tuple<int,int>)>();

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
        return coorListToStringList(coorMoves);
    }

#region Private methods

    // Converts a list of coordinates with start and end coordinate into string representation
    private List<String> coorListToStringList(List<(Tuple<int,int>, Tuple<int,int>)> coorMoves)
    {
        var moves = new List<string>();
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

    /// <summary>
    /// Returns all valid moves for a given board and piece
    /// </summary>
    /// <param name="m"> Movement pattern for piece </param>
    /// <param name = "pos"> Position of piece </parma>
    /// <param name = "size"> Length of movement pattern </param>
    /// <param name = "jump"> Is the piece allowed to jump </param>
    /// <param name = "repeat"> How many times the piece is allowed to move </param>
    private List<Tuple<int, int>> getAllValidMovesByPiece(Piece piece, Tuple<int, int> pos)
    {
        if (piece.MovementPattern is JumpMovementPattern)
            return generateJumpMoves(piece, pos);

        return generateRegularMoves(piece, pos);
    }

    // Generates all moves for a piece that can not jump over other pieces.
    private List<Tuple<int, int>> generateRegularMoves(Piece piece, Tuple<int,int> pos)
    {
        var moves = new List<Tuple<int, int>>();
        var movesTmp = getAllMoves(piece, pos);
        int repeat = piece.Repeat;

        moves = getAllMoves(piece, pos);
        while (repeat >= 1)
        {
            foreach (var move in movesTmp)
            {
                moves.AddRange(getAllMoves(piece, new Tuple<int, int>(move.Item1, move.Item2)));
            }
            repeat--;
        }
        return moves;
    }

    // Generates all moves for a piece that can jump over other pieces.
    private List<Tuple<int, int>> generateJumpMoves(Piece piece, Tuple<int,int> pos)
    {
        var moves = new List<Tuple<int, int>>();
        var movesTmp = getAllMovesJump(piece, pos);
        int repeat = piece.Repeat;

        moves = getAllMovesJump(piece, pos);
        while (repeat >= 1)
        {
            foreach (var move in movesTmp)
            {
                moves.AddRange(getAllMovesJump(piece, new Tuple<int, int>(move.Item1, move.Item2)));
                repeat--;
            }
        }
        return moves;
    }

    /// <summary>
    /// Returns all valid moves for a given board and piece that can jump
    /// </summary>
    /// <param name="m"> Movement pattern for piece </param>
    /// <param name = "pos"> Position of piece </parma>
    private List<Tuple<int, int>> getAllMovesJump(Piece piece, Tuple<int, int> pos)
    {
        var moves = new List<Tuple<int, int>>();
        for (int i = 0; i < piece.MovementPattern.Movement.Count; i++)
        {
            int newRow = pos.Item1 + piece.MovementPattern.Movement[i].Item1;
            int newCol = pos.Item2 + piece.MovementPattern.Movement[i].Item2;

            string? pieceIdentifier1 = board.GetPieceIdentifier(pos);
            string? pieceIdentifier2 = board.GetPieceIdentifier(newRow, newCol);

            if(pieceIdentifier1 == null || pieceIdentifier2 == null)
                continue;

            if(pieceIdentifier2.Equals(Constants.UnoccupiedSquareIdentifier))
            {
                moves.Add(new Tuple<int, int>(newRow, newCol));
                continue;
            }

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

    /// <summary>
    /// Returns all valid moves for a given board and piece that cannot jump
    /// </summary>
    /// <param name="m"> Movement pattern for piece </param>
    /// <param name = "pos"> Position of piece </parma>
    /// <param name = "size"> Length of movement pattern </param>
    private List<Tuple<int, int>> getAllMoves(Piece piece, Tuple<int, int> pos)
    {

        var moves = new List<Tuple<int, int>>();
        int maxIndex = Math.Max(board.Rows,board.Cols);

        for (int i = 0; i < piece.MovementPattern.Movement.Count; i++)
        {
            for (int j = 1; j < maxIndex; j++)
            {
                int newRow = pos.Item1 + piece.MovementPattern.Movement[i].Item1 * j;
                int newCol = pos.Item2 + piece.MovementPattern.Movement[i].Item2 * j;
                if(!insideBoard(newRow, newCol))
                    break;
                string? piece1 = board.GetPieceIdentifier(pos);
                string? piece2 = board.GetPieceIdentifier(newRow, newCol);

                if(piece1 != null && piece2 != null && !hasTaken(piece, pos))
                {
                    if(piece2.Equals(Constants.UnoccupiedSquareIdentifier) && (piece.MovementPattern.MoveLength[i].Item2 >= j && j >= piece.MovementPattern.MoveLength[i].Item1))
                    {
                        moves.Add(new Tuple<int, int>(newRow, newCol));
                        continue;
                    }
                    try
                    {
                        Piece p2 = this.stringToPiece[piece2];
                        if (piece.MovementPattern.MoveLength[i].Item2 >= j && j >= piece.MovementPattern.MoveLength[i].Item1 && canTake(piece, p2))
                        {
                            moves.Add(new Tuple<int, int>(newRow, newCol));
                            break;
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    catch (KeyNotFoundException) {}

                }
            }
        }
        return moves;    
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