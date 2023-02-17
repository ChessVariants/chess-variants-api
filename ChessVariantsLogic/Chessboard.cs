namespace ChessVariantsLogic;

public class Chessboard
{

    private int rows, cols;
    private char[,] board;

    public static Dictionary<string, (int, int)> CoorToIndex = initDictionary();

    public Chessboard(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;
        this.board = new char[rows, cols];
    }

    private static Dictionary<string, (int,int)> initDictionary()
    {
        var temp = new Dictionary<string, (int, int)>();

        string files = "abcdefghijklmnopqrst";

        for(int i = 0; i < 20; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                int rank = i+1;
                string notation = files[j] + rank.ToString();
                Console.WriteLine("notation: " + notation + " square: " + (i,j));
                temp.Add(notation, (i,j));
            }
        }

        return temp;
    }

    // Updates chessboard
    // @param move is the string representation of a move
    public void MakeMove(string move)
    {
        string from, to;
        (from, to) = parseMove(move);

        (int, int) fromIndex = CoorToIndex[from];
        (int, int) toIndex = CoorToIndex[to];
        
        Console.WriteLine("piece: " + (fromIndex.Item1,fromIndex.Item2));
        char piece = board[fromIndex.Item1, fromIndex.Item2];
        
        board[toIndex.Item1, toIndex.Item2] = piece;
        board[fromIndex.Item1, fromIndex.Item2] = '-';
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
                chessboard.board[i,j] = '-';
            }
        }

        // small letters black pieces, capitalized letters white pieces
        chessboard.board[0, 0] = 'R';
        chessboard.board[0, 1] = 'N';
        chessboard.board[0, 2] = 'B';
        chessboard.board[0, 3] = 'Q';
        chessboard.board[0, 4] = 'K';
        chessboard.board[0, 5] = 'B';
        chessboard.board[0, 6] = 'N';
        chessboard.board[0, 7] = 'R';

        chessboard.board[7, 0] = 'r';
        chessboard.board[7, 1] = 'n';
        chessboard.board[7, 2] = 'b';
        chessboard.board[7, 3] = 'q';
        chessboard.board[7, 4] = 'k';
        chessboard.board[7, 5] = 'b';
        chessboard.board[7, 6] = 'n';
        chessboard.board[7, 7] = 'r';

        // pawns
        for (int i = 0; i < 8; i++)
        {
            chessboard.board[6, i] = 'p';
            chessboard.board[1, i] = 'P';
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
