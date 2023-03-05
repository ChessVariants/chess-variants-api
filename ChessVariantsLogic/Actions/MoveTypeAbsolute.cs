using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Actions;
public class MoveTypeAbsolute : IMoveType
{
    private readonly string _fromTo;

    public MoveTypeAbsolute(string fromTo)
    {
        _fromTo = fromTo;
    }

    public string GetFromTo(IBoardState moveWorker)
    {
        return _fromTo;
    }
}
