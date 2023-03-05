using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Actions;
public interface IMoveType
{
    public string GetFromTo(IBoardState moveWorker);
}
