namespace ChessVariantsLogic;
using static Piece;
using static Chessboard;

public class GameDriver
{
    private Chessboard board;

    public Chessboard Board
    {
        get { return this.board; }
        set { this.board = value; }
    }

    private readonly Dictionary<string, Piece> stringToPiece;
    
    public GameDriver(Chessboard chessboard)
    {
        this.board = chessboard;
        stringToPiece = initStringToPiece();
    }

    /// <summary>
    /// Updates the chessboard by moving the square from the first coordinate to the last coordinate in move. The first coordinate will be marked as unoccupied.
    /// </summary>
    /// <param name="move"> consists of two coordinates without any space between them. </param>
    /// <returns> true if move was valid and successful. </returns>
    public bool Move(string move)
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
                    return true;   
                }
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
        return false;

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
        if (piece.Jump)
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
        for (int i = 0; i < piece.Movement.Count; i++)
        {
            int newRow = pos.Item1 + piece.Movement[i].Item1;
            int newCol = pos.Item2 + piece.Movement[i].Item2;

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

        for (int i = 0; i < piece.Movement.Count; i++)
        {
            for (int j = 1; j < maxIndex; j++)
            {
                int newRow = pos.Item1 + piece.Movement[i].Item1 * j;
                int newCol = pos.Item2 + piece.Movement[i].Item2 * j;
                if(!insideBoard(newRow, newCol))
                {
                    break;
                }
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
                        if (piece.MoveLength.Item2 >= j && j >= piece.MoveLength.Item1 && canTake(p1, p2))
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

        dictionary.Add(Constants.BlackRookIdentifier, Piece.Rook(PieceClassifier.BLACK));
        dictionary.Add(Constants.BlackKnightIdentifier, Piece.Knight(PieceClassifier.BLACK));
        dictionary.Add(Constants.BlackBishopIdentifier, Piece.Bishop(PieceClassifier.BLACK));
        dictionary.Add(Constants.BlackKingIdentifier, Piece.King(PieceClassifier.BLACK));
        dictionary.Add(Constants.BlackQueenIdentifier, Piece.Queen(PieceClassifier.BLACK));
        dictionary.Add(Constants.BlackPawnIdentifier, Piece.BlackPawn());

        dictionary.Add(Constants.WhiteRookIdentifier, Piece.Rook(PieceClassifier.WHITE));
        dictionary.Add(Constants.WhiteKnightIdentifier, Piece.Knight(PieceClassifier.WHITE));
        dictionary.Add(Constants.WhiteBishopIdentifier, Piece.Bishop(PieceClassifier.WHITE));
        dictionary.Add(Constants.WhiteKingIdentifier, Piece.King(PieceClassifier.WHITE));
        dictionary.Add(Constants.WhiteQueenIdentifier, Piece.Queen(PieceClassifier.WHITE));
        dictionary.Add(Constants.WhitePawnIdentifier, Piece.WhitePawn());

        return dictionary;
    }

}