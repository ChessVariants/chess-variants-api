using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessVariantsLogic.Rules.Predicates;
public class PredicateParser
{

    private IDictionary<string, string> variables = new Dictionary<string, string>();

    public IPredicate ParseCode(string code)
    {
        code = RemoveSpacesAndNewLines(code + '\n');
        StringBuilder variable = new StringBuilder();
        StringBuilder value = new StringBuilder();
        bool shouldWriteToVariable = true;
        foreach(char c in code)
        {
            if(c.Equals('='))
            {
                shouldWriteToVariable = false;
            }
            else if(c.Equals('\n'))
            {
                if (variable.Length == 0)
                    continue;
                shouldWriteToVariable = true;
                variables.Add(variable.ToString(), value.ToString());
                variable.Clear();
                value.Clear();
            }
            else if(shouldWriteToVariable)
            {
                variable.Append(c);
            }
            else
            {
                value.Append(c);
            }
        }
        variables.TryGetValue("return", out string? predicate);
        
        if (predicate == null)
            throw new ArgumentException("Code does not contain a return value");

        string finalPredicate = GetFinalPredicate(predicate);

        return ParsePredicate(finalPredicate);

    }

    private string ReplaceWord(string input, string wordToFind, string replace)
    {
        string pattern = string.Format(@"\b{0}\b", wordToFind);
        return Regex.Replace(input, wordToFind, replace);
    }

    private string GetFinalPredicate(string predicate)
    {
        while (ContainsAnyVariable(predicate))
        {
            foreach (string variableName in variables.Keys)
            {
                if(Regex.Match(predicate, string.Format(@"\b{0}\b", variableName)).Success)
                {
                    predicate = ReplaceWord(predicate, variableName, variables[variableName]);
                }
            }
        }
        return predicate;
    }

    private bool ContainsAnyVariable(string predicate)
    {
        foreach(string variableName in variables.Keys)
        {
            if(Regex.Match(predicate, string.Format(@"\b{0}\b", variableName)).Success)
            {
                return true;
            }
        }
        return false;
    }

    public IPredicate ParsePredicate(string pred)
    {
        pred = RemoveSpacesAndNewLines(pred);
        Tuple<string, List<string>> function = GetFunction(pred);
        (var functionName, _) = function;
        
        if(IsOperator(functionName))
            return ParseOperator(function);
        if(IsChessPredicate(functionName))
            return ParseChessPredicate(function);
        if(IsConstant(pred))
            return ParseConst(pred);
        
        throw new ArgumentException("Unknown identifier: " + pred);
    }

    private bool IsConstant(string pred)
    {
        return pred switch
        {
            "true" => true,
            "false" => true,
            _ => false,
        };
    }

    private IPredicate ParseConst(string pred)
    {
        return pred switch
        {
            "true" => new Const(true),
            "false" => new Const(false),
            _ => throw new ArgumentException("invalid argument"),
        };
    }

    private bool IsOperator(string word)
    {
        return word switch
        {
            "OR" => true,
            "AND" => true,
            "IMPLIES" => true,
            "XOR" => true,
            "EQUALS" => true,
            "NOT" => true,
            _ => false,
        };
    }
    private bool IsChessPredicate(string word)
    {
        return word switch
        {
            "movenpred" => true,
            "squarenpred" => true,
            "piecenpred" => true,
            "countnpred" => true,
            _ => false,
        };
    }


    private IPredicate ParseOperator(Tuple<string, List<string>> operatorFunction)
    {
        (var operatorType, var args) = operatorFunction;
        if (operatorType == "NOT")
            return !ParsePredicate(args[0]);


        IPredicate arg1 = ParsePredicate(args[0]);
        IPredicate arg2 = ParsePredicate(args[1]);

        return operatorType switch
        {
            "OR" => (arg1 | arg2),
            "AND" => (arg1 & arg2),
            "IMPLIES" => (arg1 - arg2),
            "XOR" => (arg1 ^ arg2),
            "EQUALS" => new Operator(arg1, OperatorType.EQUALS, arg2),
            _ => throw new ArgumentException("Invalid operator: " + operatorType),
        };
    }


    private IPredicate ParseChessPredicate(Tuple<string, List<string>> predicate)
    {
        (var predType, var args) = predicate;

        return predType switch
        {
            "movenpred" => ParseMovePred(args),
            "piecenpred" => ParsePiecePred(args),
            "squarenpred" => ParseSquarePred(args),
            _ => throw new ArgumentException("u done messed up"),
        };
    }

    private IPredicate ParseMovePred(List<string> args)
    {
        string type = args[0];
        string info = args[1];
        string state = args[2];
        MoveState moveState = ParseMoveState(state);
        return type switch
        {
            "captured" => new PieceCaptured(info),
            "name" => new PieceMoved(info, moveState),
            _ => throw new ArgumentException("Invalid type argument of move_pred function: " + type),
        };
    }

    private MoveState ParseMoveState(string state)
    {
        if (state == "this")
            return MoveState.THIS;
        else if (state == "last")
            return MoveState.LAST;
        else
            throw new ArgumentException("Invalid move_state variable: " + state);
    }

    private IPredicate ParsePiecePred(List<string> args)
    {
        string type = args[0];
        string piece = args[1];
        string state = args[2];
        BoardState boardState = state == "this" ? BoardState.THIS : (state == "next" ? BoardState.NEXT : throw new ArgumentException("Invalid state argument of piece_pred function: " + state));
        return type switch
        {
            "attacked" => new Attacked(boardState, piece),
            _ => throw new ArgumentException("Invalid type argument of move_pred function: " + type),
        };
    }

    private IPredicate ParseSquarePred(List<string> args)
    {
        string state = args[0];
        string relative_to = args[1];
        string position = args[2];
        string type = args[3];
        string info = "";
        if (!type.Equals("hasnmoved"))
            info = args[4];
        var boardState = ParseBoardState(state);
        var relativeTo = ParseRelativeTo(relative_to);
        var iPosition = ParsePosition(position);
        switch (type)
        {
            case "attackednby":
                {
                    var player = ParsePlayer(info);
                    return new SquareAttacked(iPosition, boardState, player, relativeTo);
                }
            case "hasnmoved":
                {
                    return new HasMoved(iPosition, boardState, relativeTo);
                }
            case "hasnpiece":
                {
                    return new PieceAt(info, iPosition, boardState, relativeTo);
                }
            case "hasnrank":
                {
                    return new PositionHasRank(iPosition, int.Parse(info), relativeTo);
                }
            case "hasnfile":
                throw new NotImplementedException();
            case "is":
                throw new NotImplementedException();
            default:
                throw new ArgumentException("Invalid type argument of square_pred function: " + type);
        };
    }

    private static RelativeTo ParseRelativeTo(string relative_to)
    {
        if (relative_to == "from")
            return RelativeTo.FROM;
        else if (relative_to == "to")
            return RelativeTo.TO;
        else
            throw new ArgumentException("Invalid relative_to argument of piece_pred function: " + relative_to);
    }

    private Player ParsePlayer(string info)
    {

        if (info == "black")
            return Player.Black;
        else if (info == "white")
            return Player.White;
        else if(info == "shared") 
            return Player.None;
        else
            throw new ArgumentException("Invalid player variable: " + info);
    }

    private static BoardState ParseBoardState(string state)
    {
        if (state == "this")
            return BoardState.THIS;
        else if (state == "next")
            return BoardState.NEXT;
        else
            throw new ArgumentException("Invalid state variable: " + state);
    }

    private IPosition ParsePosition(string position)
    {
        var func = GetFunction(position);
        (var functionName, var args) = func;
        switch(functionName)
        {
            case "absolute":
                return new PositionAbsolute(args[0]);
            case "relative":
                return new PositionRelative(int.Parse(args[1]), int.Parse(args[2]));
            default:
                throw new ArgumentException("Invalid position: " + position);
        }

    }

    private Tuple<string, List<string>> GetFunction(string function)
    {

        string functionName = GetFirstWord(function);
        if (functionName.Length == function.Length)
            return new Tuple<string, List<string>>(functionName, new List<string>());
        string functionBody = SubstringFromTo(function, functionName.Length + 1, function.Length - 1);
        StringBuilder arg = new StringBuilder();
        List<string> args = new List<string>();
        int paranthesis = 0;
        foreach (char c in functionBody)
        {

            if (c.Equals(',') && paranthesis == 0)
            {
                args.Add(arg.ToString());
                arg.Clear();
                continue;
            }

            arg.Append(c);

            if (c.Equals('('))
            {
                paranthesis++;
            }
            else if (c.Equals(')'))
            {
                paranthesis--;
            }
        }
        if (paranthesis == 0)
        {
            args.Add(arg.ToString());
            return new Tuple<string, List<string>>(functionName, args);
        }

        throw new ArgumentException("function isn't a proper function");
    }
    private string GetFirstWord(string line)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(char c in line)
        {
            
            if(char.IsLetter(c) || c.Equals('_'))
            {
                stringBuilder.Append(c);
            }
            else
            {
                return stringBuilder.ToString();
            }
        }
        return stringBuilder.ToString();
    }

    private string RemoveSpacesAndNewLines(string input)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (char c in input)
        {
            if (c.Equals(' '))
                continue;
            if(c.Equals('_'))
                stringBuilder.Append('n');
            else
                stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }
    private string SubstringFromTo(string str, int start, int end)
    {
        return str.Substring(start, end - start);
    }


}
