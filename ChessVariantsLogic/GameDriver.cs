namespace ChessVariantsLogic;
using static Piece;

public class GameDriver
{
    private Chessboard board;

    private readonly HashSet<Piece> pieces;

    public Chessboard Board
    {
        get { return this.board; }
        set { this.board = value; }
    }

    private readonly Dictionary<string, Piece> stringToPiece;
    
    public GameDriver(Chessboard chessboard, HashSet<Piece> pieces)
    {
        this.board = chessboard;
        this.pieces = pieces;
        stringToPiece = initStringToPiece();
    }

    public GameDriver(Chessboard chessboard) : this(chessboard, new HashSet<Piece>()) {}

    /// <summary>
    /// Updates the chessboard by moving the square from the first coordinate to the last coordinate in move. The first coordinate will be marked as unoccupied.
    /// </summary>
    /// <param name="move"> consists of two coordinates without any space between them. </param>
    /// <returns> A GameEvent representing whether the move was successful or not. </returns>
    public GameEvent Move(string move)
    {
        var (from, to) = parseMove(move);
        
        string? strPiece = this.board.GetPieceAsString(from);
        if(strPiece != null)
        {
            try
            {
                Piece piece = stringToPiece[strPiece];
                var moves = getAllValidMoves(piece, this.board, this.board.CoorToIndex[from]);
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
    private (string, string) parseMove(string move)
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
    /// Returns all valid moves for a given board and piece
    /// </summary>
    /// <param name="m"> Movement pattern for piece </param>
    /// <param name = "board"> Current board state </param>
    /// <param name = "pos"> Position of piece </parma>
    /// <param name = "size"> Length of movement pattern </param>
    /// <param name = "jump"> Is the piece allowed to jump </param>
    /// <param name = "repeat"> How many times the piece is allowed to move </param>
    public List<Tuple<int, int>> getAllValidMoves(Piece piece, Chessboard board, Tuple<int, int> pos)
    {
        
        int repeat = piece.Repeat;
        var moves = new List<Tuple<int, int>>();
        
        if (piece.MovementPattern is JumpMovementPattern)
        {
            var movesTmp = getAllMovesJump(piece, board, pos);
            moves = getAllMovesJump(piece, board, pos);
            while (repeat >= 1)
            {
                foreach (var move in movesTmp)
                {
                    moves.AddRange(getAllMovesJump(piece, board, new Tuple<int, int>(move.Item1, move.Item2)));
                    repeat--;
                }
            }
        }
        else
        {
            var movesTmp = getAllMoves(piece, board, pos);
            moves = getAllMoves(piece, board, pos);
            while (repeat >= 1)
            {
                foreach (var move in movesTmp)
                {
                    moves.AddRange(getAllMoves(piece, board, new Tuple<int, int>(move.Item1, move.Item2)));
                    repeat--;
                }
            }
        }
        return moves;
    }

    /// <summary>
    /// Returns all valid moves for a given board and piece that can jump
    /// </summary>
    /// <param name="m"> Movement pattern for piece </param>
    /// <param name = "board"> Current board state </param>
    /// <param name = "pos"> Position of piece </parma>
    private List<Tuple<int, int>> getAllMovesJump(Piece piece, Chessboard board, Tuple<int, int> pos)
    {
        var moves = new List<Tuple<int, int>>();
        for (int i = 0; i < piece.MovementPattern.Movement.Count; i++)
        {
            int newRow = pos.Item1 + piece.MovementPattern.Movement[i].Item1;
            int newCol = pos.Item2 + piece.MovementPattern.Movement[i].Item2;

            string? piece1 = board.GetPieceAsString(pos);
            string? piece2 = board.GetPieceAsString(newRow, newCol);

            if(piece1 != null && piece2 != null)
            {
                if(piece2.Equals(Constants.UnoccupiedSquareIdentifier))
                    {
                        moves.Add(new Tuple<int, int>(newRow, newCol));
                        continue;
                    }
                try
                {
                    Piece p1 = this.stringToPiece[piece1];
                    Piece p2 = this.stringToPiece[piece2];
                    if (insideBoard(newRow, newCol) && canTake(p1, p2))
                        moves.Add(new Tuple<int, int>(newRow, newCol));
                }
                catch (KeyNotFoundException) {}
            }

        }
        return moves;
    }

    /// <summary>
    /// Returns all valid moves for a given board and piece that cannot jump
    /// </summary>
    /// <param name="m"> Movement pattern for piece </param>
    /// <param name = "board"> Current board state </param>
    /// <param name = "pos"> Position of piece </parma>
    /// <param name = "size"> Length of movement pattern </param>
    private List<Tuple<int, int>> getAllMoves(Piece piece, Chessboard board, Tuple<int, int> pos)
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
                string? piece1 = board.GetPieceAsString(pos);
                string? piece2 = board.GetPieceAsString(newRow, newCol);
                if(piece1 != null && piece2 != null)
                {
                    if(piece2.Equals(Constants.UnoccupiedSquareIdentifier) && (piece.MovementPattern.MoveLength[i].Item2 >= j && j >= piece.MovementPattern.MoveLength[i].Item1))
                    {
                        moves.Add(new Tuple<int, int>(newRow, newCol));
                        continue;
                    }
                    try
                    {
                        Piece p1 = this.stringToPiece[piece1];
                        Piece p2 = this.stringToPiece[piece2];
                        if (piece.MovementPattern.MoveLength[i].Item2 >= j && j >= piece.MovementPattern.MoveLength[i].Item1 && canTake(p1, p2))
                        {
                            moves.Add(new Tuple<int, int>(newRow, newCol));
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

}

public enum GameEvent {
    InvalidMove,
    MoveSucceeded,
    WhiteWon,
    BlackWon,
    Tie
}
