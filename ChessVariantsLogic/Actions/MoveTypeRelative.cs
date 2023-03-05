using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Actions;
public class MoveTypeRelative : IMoveType
{
    private readonly Piece _piece;
    private readonly Tuple<int, int> _to;

    public MoveTypeRelative(Piece piece, Tuple<int, int> to)
    {
        _piece = piece;
        _to = to;
    }

    public string GetFromTo(IBoardState moveWorker)
    {
        Tuple<int, int> position = Tuple.Create(0, 0); // _piece.GetPosition();

        string? from;
        string? to;

        moveWorker.Board.IndexToCoor.TryGetValue(position, out from);

        position = new Tuple<int, int>(_to.Item1 + position.Item1, _to.Item2 + position.Item2);

        moveWorker.Board.IndexToCoor.TryGetValue(position, out to);
        return from + to;
    }
}
