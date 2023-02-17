namespace ChessVariantsLogic;

public class Chessboard
{

    private int rows, cols;
    private string[,] board;

    public Chessboard(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        this.board = new string[rows, cols];
    }

    // Updates chessboard
    // @param move is the string representation of a move
    public void MakeMove(string move)
    {
        string from, to;
        (from, to) = parseMove(move);

        (int, int) fromIndex = Constants.CoorToIndex[from];
        (int, int) toIndex = Constants.CoorToIndex[to];

        string piece = board[fromIndex.Item1, fromIndex.Item2];
        
        board[toIndex.Item1, toIndex.Item2] = piece;
        board[fromIndex.Item1, fromIndex.Item2] = "-";
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

    public static Chessboard StandardChessboard()
    {
        var chessboard = new Chessboard(8, 8);

        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                chessboard.board[i,j] = Constants.UnoccupiedSquareIdentifier;
            }
        }

        // small letters black pieces, capitalized letters white pieces
        chessboard.board[0, 0] = Constants.WhiteRookIdentifier;
        chessboard.board[0, 1] = Constants.WhiteKnightIdentifier;
        chessboard.board[0, 2] = Constants.WhiteBishopIdentifier;
        chessboard.board[0, 3] = Constants.WhiteQueenIdentifier;
        chessboard.board[0, 4] = Constants.WhiteKingIdentifier;
        chessboard.board[0, 5] = Constants.WhiteBishopIdentifier;
        chessboard.board[0, 6] = Constants.WhiteKnightIdentifier;
        chessboard.board[0, 7] = Constants.WhiteRookIdentifier;

        chessboard.board[7, 0] = Constants.BlackRookIdentifier;
        chessboard.board[7, 1] = Constants.BlackKnightIdentifier;
        chessboard.board[7, 2] = Constants.BlackBishopIdentifier;
        chessboard.board[7, 3] = Constants.BlackQueenIdentifier;
        chessboard.board[7, 4] = Constants.BlackKingIdentifier;
        chessboard.board[7, 5] = Constants.BlackBishopIdentifier;
        chessboard.board[7, 6] = Constants.BlackKnightIdentifier;
        chessboard.board[7, 7] = Constants.BlackRookIdentifier;

        // pawns
        for (int i = 0; i < 8; i++)
        {
            chessboard.board[6, i] = Constants.BlackPawnIdentifier;
            chessboard.board[1, i] = Constants.WhitePawnIdentifier;
        }
        return chessboard;
    }
    
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
}
