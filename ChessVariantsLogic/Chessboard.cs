namespace ChessVariantsLogic;

/// <summary>
/// Class representing a chessboard that can be initialized as a rectangle of max size 20X20. Currently uses strings to represent pieces.
/// </summary>
public class Chessboard
{

/// <summary>
    /// Returns all valid moves for a given board and piece
    /// </summary>
    /// <param name="m"> Movement pattern for piece </param>
    /// <param name = "board"> Current board state </param>
    /// <param name = "pos"> Position of piece </parma>
    /// <param name = "size"> Length of movement pattern </param>
    /// <param name = "jump"> Is the piece allowed to jump </param>
    /// <param name = "repeat"> How many times the piece is allowed to move </param>
    static List<Tuple<int, int>> getAllValidMoves((int, int)[] m, int[,] board, (int, int) pos, (int, int) size, bool jump, int repeat)
    {
        var moves = new List<Tuple<int, int>>();
        if (jump)
        {
            moves = getAllMovesJump(m, board, pos, size);
        }
        else
        {
            var movesTmp = getAllMoves(m, board, pos, size);
            moves = getAllMoves(m, board, pos, size);
            while (repeat >= 1)
            {
                foreach (var move in movesTmp)
                {
                    moves.AddRange(getAllMoves(m, board, (move.Item1, move.Item2), size));
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
    
    
    
    static List<Tuple<int, int>> getAllMovesJump((int, int)[] m, int[,] board, (int, int) pos)
    {
        var moves = new List<Tuple<int, int>>();
        for (int i = 0; i < m.Length; i++)
        {
            int newRow = pos.Item1 + m[i].Item1;
            int newCol = pos.Item2 + m[i].Item2;
            if ((0 <= newRow && newRow <= board.GetLength(0)) && (0 <= newCol && newCol <= board.GetLength(0)))
            {
                moves.Add(new Tuple<int, int>(newRow, newCol));
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
   
    static List<Tuple<int, int>> getAllMoves((int, int)[] m, int[,] board, (int, int) pos, (int, int) size)
    {

        var moves = new List<Tuple<int, int>>();

        for (int i = 0; i < m.Length; i++)
        {
            for (int j = 1; j < board.GetLength(0); j++)
            {
                int newRow = pos.Item1 + m[i].Item1 * j;
                int newCol = pos.Item2 + m[i].Item2 * j;

                if ((0 <= newRow && newRow <= board.GetLength(0) - 1) && (0 <= newCol && newCol <= board.GetLength(0) - 1))
                {
                    if (size.Item2 >= j && j >= size.Item1 && board[pos.Item1, pos.Item2] == 0)
                    {
                        moves.Add(new Tuple<int, int>(newRow, newCol));
                    }
                    if (board[newRow, newCol] == 2) break;

                }
            }
        }
        return moves;
    }

}
