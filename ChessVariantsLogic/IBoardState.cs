
using System.Collections.Generic;

namespace ChessVariantsLogic;
public interface IBoardState
{

    public GameEvent Move(string move);

    public List<string> GetAllValidMoves(Player player);

    public Tuple<string, string>? parseMove(string move);

    public Chessboard Board
    {
        get { return Chessboard.StandardChessboard(); }
        set { }
    }

    public IBoardState CopyBoardState();
}