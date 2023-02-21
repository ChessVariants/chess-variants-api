namespace ChessVariantsLogic;

/// <summary>
/// Class representing a chessboard that can be initialized as a rectangle of max size 20X20. Currently uses strings to represent pieces.
/// </summary>
public class Chessboard
{

    private readonly int rows;
    private readonly int cols;
    private readonly string[,] board;

    private readonly Dictionary<string, (int, int)> coorToIndex;

    /// <summary>
    /// Maps a string representation of a square to its corresponding index on the board.
    /// </summary>
    public Dictionary<string, (int, int)> CoorToIndex
    {
        get { return this.coorToIndex; }
    }

    public Chessboard(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        this.board = new string[rows, cols];
        coorToIndex = initDictionary();
        board = initBoard();
    }

    public Chessboard(int length) : this(length, length) {}

    /// <summary>
    /// Updates the chessboard by moving the square from the first coordinate to the last coordinate in move. The first coordinate will be marked as unoccupied.
    /// </summary>
    /// <param name="move"> consists of two coordinates without any space between them. </param>
    public bool Move(string move)
    {
        string from, to;
        (from, to) = parseMove(move);

        try
        {
            (int, int) fromIndex = coorToIndex[from];
            (int, int) toIndex = coorToIndex[to];
            string piece = board[fromIndex.Item1, fromIndex.Item2];
        
            board[toIndex.Item1, toIndex.Item2] = piece;
            board[fromIndex.Item1, fromIndex.Item2] = Constants.UnoccupiedSquareIdentifier;
            return true;
        }
        catch(KeyNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// Produces FEN representation of the chessboard
    /// </summary>
    /// <returns> a string representing the chessboard in FEN </returns>
    public string ReadBoardAsFEN()
    {
        string fen = "";
        int unoccupiedCounter = 0;

        for(int i = 0; i < this.rows; i++)
        {
            for(int j = 0; j < this.cols; j++)
            {
                if(board[i,j].Equals(Constants.UnoccupiedSquareIdentifier))
                {
                    unoccupiedCounter++;
                    continue;
                }
                if (unoccupiedCounter != 0)
                {
                    fen += unoccupiedCounter.ToString();
                    unoccupiedCounter = 0;
                }
                fen += board[i,j];
            }
            if(unoccupiedCounter != 0)
            {
                fen += unoccupiedCounter.ToString();
                unoccupiedCounter = 0;
            }
            fen += "/";
        }

        return fen.Remove(fen.Length - 1, 1);
    }

    /// <returns> an instance of Chessboard with the standard set up. </returns>
    public static Chessboard StandardChessboard()
    {
        var chessboard = new Chessboard(8);

        chessboard.board[0, 0] = Constants.BlackRookIdentifier;
        chessboard.board[0, 1] = Constants.BlackKnightIdentifier;
        chessboard.board[0, 2] = Constants.BlackBishopIdentifier;
        chessboard.board[0, 3] = Constants.BlackQueenIdentifier;
        chessboard.board[0, 4] = Constants.BlackKingIdentifier;
        chessboard.board[0, 5] = Constants.BlackBishopIdentifier;
        chessboard.board[0, 6] = Constants.BlackKnightIdentifier;
        chessboard.board[0, 7] = Constants.BlackRookIdentifier;
        
        chessboard.fillRank(1, Constants.BlackPawnIdentifier);
        chessboard.fillRank(2, Constants.UnoccupiedSquareIdentifier);
        chessboard.fillRank(3, Constants.UnoccupiedSquareIdentifier);
        chessboard.fillRank(4, Constants.UnoccupiedSquareIdentifier);
        chessboard.fillRank(5, Constants.UnoccupiedSquareIdentifier);
        chessboard.fillRank(6, Constants.WhitePawnIdentifier);
        
        chessboard.board[7, 0] = Constants.WhiteRookIdentifier;
        chessboard.board[7, 1] = Constants.WhiteKnightIdentifier;
        chessboard.board[7, 2] = Constants.WhiteBishopIdentifier;
        chessboard.board[7, 3] = Constants.WhiteQueenIdentifier;
        chessboard.board[7, 4] = Constants.WhiteKingIdentifier;
        chessboard.board[7, 5] = Constants.WhiteBishopIdentifier;
        chessboard.board[7, 6] = Constants.WhiteKnightIdentifier;
        chessboard.board[7, 7] = Constants.WhiteRookIdentifier;

        return chessboard;
    }

#region Getters and setters
    /// <summary>
    /// Inserts the piece onto the square on the chessboard.
    /// </summary>
    /// <param name="pieceIdentifier"> the piece to be inserted </param>
    /// <param name="row"> the row index </param>
    /// <param name="col"> the column index </param>
    /// <returns> true if piece was successfully inserter into the square, otherwise false. </returns>
    public bool Insert(string pieceIdentifier, int row, int col)
    {
        if(validIndex(row, col))
        {
            board[row,col] = pieceIdentifier;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Inserts the piece onto the square on the chessboard.
    /// </summary>
    /// <param name="pieceIdentifier"> the piece to be inserted </param>
    /// <param name="index"> is the index of the square as a tuple of ints. </param>
    /// <returns> true if piece was successfully inserter into the square, otherwise false. </returns>
    public bool Insert(string pieceIdentifier, (int, int) index)
    {
        return Insert(pieceIdentifier, index.Item1, index.Item2);
    }

    /// <summary>
    /// Inserts the piece onto the square on the chessboard.
    /// </summary>
    /// <param name="pieceIdentifier"> is the piece to be inserted. </param>
    /// <param name="coordinate"> is the coordinate of the square as a string. </param>
    /// <returns> true if piece was successfully inserter into the square, otherwise false. </returns>
    public bool Insert(string pieceIdentifier, string coordinate)
    {
        try {
            var key = this.coorToIndex[coordinate];
            return Insert(pieceIdentifier, key.Item1, key.Item2);
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the piece occupying the requested square if the square is inside the bounds of the chessboard.
    /// </summary>
    /// <param name="row"> is the row index. </param>
    /// <param name="col"> is the column index. </param>
    /// <returns> the piece if the index is valid. </returns>
    public string? GetPiece(int row, int col)
    {
        if(validIndex(row, col))
            return board[row,col];
        return null;
    }

    /// <summary>
    /// Gets the piece occupying the requested square if the square is inside the bounds of the chessboard.
    /// </summary>
    /// <param name="index"> is the index of the square as a tuple of ints </param>
    /// <returns> the piece if the index is valid. </returns>
    public string? GetPiece((int, int) index)
    {
        return GetPiece(index.Item1, index.Item2);
    }

    /// <summary>
    /// Gets the piece occupying the requested square if the square is inside the bounds of the chessboard.
    /// </summary>
    /// <param name="coordinate"> is the coordinate of the square as a string </param>
    /// <returns> the piece if the index is valid. </returns>
    public string? GetPiece(string coordinate)
    {
        try
        {
            var key = this.coorToIndex[coordinate];
            return GetPiece(key);
        }
        catch (Exception)
        {
            return null;
        }
    }
#endregion

#region private methods
    private void fillRank(int rank, string squareIdentifier)
    {
        for(int file = 0; file < this.cols; file++)
            board[rank, file] = squareIdentifier;
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

    private Dictionary<string, (int,int)> initDictionary()
    {
        var dictionary = new Dictionary<string, (int, int)>();

        for(int i = 0; i < this.rows; i++)
        {
            for(int j = 0; j < this.cols; j++)
            {
                int rank = this.rows-i;
                string notation = Constants.BoardFiles[j] + rank.ToString();
                dictionary.Add(notation, (i,j));
            }
        }
        return dictionary;
    }

    private string[,] initBoard()
    {
        var board = new string[this.rows, this.cols];
        for(int i = 0; i < board.GetLength(0); i++)
        {
            for(int j = 0; j < board.GetLength(1); j++)
            {
                board[i,j] = Constants.UnoccupiedSquareIdentifier;
            }
        }
        return board;
    }

    private bool validIndex(int row, int col)
    {
        if(row >= 0 && row < this.rows)
        {
            if(col >= 0 && col < this.cols)
            {
                return true;
            }
        }
        return false;
    }

#endregion

#region Overrides

    /// <summary>
    /// Creates a readble string representation of the board.
    /// </summary>
    /// <returns> a string representing the board. </returns>
    public override string ToString()
    {
        string strBoard = "";
        for(int row = 0; row < this.rows; row++)
        {
            for(int col = 0; col < this.cols; col++)
            {
                strBoard += board[row,col] + ", ";
            }
            strBoard += "\n";
        }
        return strBoard;
    }

#endregion

}
