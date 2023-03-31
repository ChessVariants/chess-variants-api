using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessVariantsLogic.Rules.Predicates.ChessPredicates.NewPredicates;
public class PiecePredicate : IPredicate
{
    [JsonProperty]
    private readonly string _pieceIdentifier;
    [JsonProperty]
    private readonly PiecePredType _type;


    public bool Evaluate(BoardTransition transition)
    {
        bool evaluation = false;
        switch(_type)
        {
            case PiecePredType.CAPTURED:
                {
                    int amountOfPieces = Utils.FindPiecesOfType(transition.ThisState, _pieceIdentifier).Count();
                    int amountOfPieces_nextState = Utils.FindPiecesOfType(transition.NextState, _pieceIdentifier).Count();
                    int diff = amountOfPieces - amountOfPieces_nextState;
                    evaluation = diff > 0;
                    break;
                }
            case PiecePredType.MOVED:
                {
                    string? piece = transition.ThisState.Board.GetPieceIdentifier(transition.MoveFrom);
                    if(piece != null ) 
                        evaluation = (piece == _pieceIdentifier) && (transition.MoveFrom != transition.MoveTo);
                    break;
                }
            default:
                return false;
        }
        return evaluation;
    }
}
public enum PiecePredType
{
    CAPTURED, MOVED
}
