using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessVariantsLogic.Rules.Predicates;
public class PredicateParser
{

    private IDictionary<string, string> variables = new Dictionary<string, string>();

    #region BoardStates
    private const string thisState = "this_state";
    private const string nextState = "next_state";
    private static readonly string[] boardstates = new string[] { thisState, nextState };
    #endregion

    #region Comparators
    private const string GREATER_THAN = "GREATER_THAN";
    private const string LESS_THAN = "LESS_THAN";
    private const string GREATER_THAN_OR_EQUALS = "GREATER_THAN_OR_EQUALS";
    private const string LESS_THAN_OR_EQUALS = "LESS_THAN_OR_EQUALS";
    private const string EQUALS_COMPARATOR = "EQUALS";
    private const string NOT_EQUALS = "NOT_EQUALS";
    private static readonly string[] comparators = new string[] { GREATER_THAN, LESS_THAN, GREATER_THAN_OR_EQUALS, LESS_THAN_OR_EQUALS, EQUALS_COMPARATOR, NOT_EQUALS };
    #endregion

    #region ConstValues
    private const string TRUE = "true";
    private const string FALSE = "false";
    private static readonly string[] constValues = new string[] { TRUE, FALSE };
    #endregion

    #region CountPredicateTypes
    private const string piecesLeft = "pieces_left";
    private static readonly string[] countPredicateTypes = new string[] { piecesLeft };
    #endregion

    #region MovePredicateTypes
    private const string captured = "captured";
    private const string pieceMoved = "piece_moved";
    private const string was = "was";
    private const string firstMove = "first_move";
    private static readonly string[] movePredicateTypes = new string[] { captured, pieceMoved, was, firstMove };
    #endregion

    #region MoveStates
    private const string thisMove = "this_move";
    private const string lastMove = "last_move";
    private static readonly string[] moveStates = new string[] { thisMove, lastMove };
    #endregion

    #region Operators
    private const string AND = "AND";
    private const string OR = "OR";
    private const string IMPLIES = "IMPLIES";
    private const string XOR = "XOR";
    private const string EQUALS_OPERATOR = "EQUALS";
    private const string NOT = "NOT";
    private static readonly string[] operatorTypes = new string[] { AND, OR, IMPLIES, XOR, EQUALS_OPERATOR, NOT };
    #endregion

    #region PieceClassifiers
    private const string white = "WHITE";
    private const string black = "BLACK";
    private const string shared = "SHARED";
    private static readonly string[] pieceClassifiers = new string[] { white, black, shared };
    #endregion

    #region PiecePredicateTypes
    private const string attacked = "attacked";
    private static readonly string[] piecePredicateTypes = new string[] { attacked };
    #endregion

    #region Predicates
    private const string countPredicate = "count_pred";
    private const string movePredicate = "move_pred";
    private const string piecePredicate = "piece_pred";
    private const string squarePredicate = "square_pred";
    private static readonly string[] predicateTypes = new string[] { countPredicate, movePredicate, piecePredicate, squarePredicate };
    #endregion

    #region RelativeToTypes
    private const string from = "from";
    private const string to = "to";
    private static readonly string[] relativeTo = new string[] { from, to };
    #endregion

    #region SquarePredicateTypes
    private const string attackedBy = "attacked_by";
    private const string hasMoved = "has_moved";
    private const string _is = "is";
    private const string hasPiece = "has_piece";
    private const string hasRank = "has_rank";
    private const string hasFile = "has_file";
    private static readonly string[] squarePredicateTypes = new string[] { attackedBy, hasMoved, _is, hasPiece, hasRank, hasFile };
    #endregion

    #region SquareTypes
    private const string absolute = "absolute";
    private const string relative = "relative";
    private static readonly string[] squareTypes = new string[] { absolute, relative };
    #endregion




    string[][] syntax = new string[][] { predicateTypes, operatorTypes, constValues, countPredicateTypes, comparators, movePredicateTypes, piecePredicateTypes, squarePredicateTypes, relativeTo, squareTypes, pieceClassifiers, boardstates, moveStates };

    string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHJIKLMNOPQRSTUVWXYZ;(),[]&|!=>_^\n";

    private void CheckForInvalidCharacters(string code)
    {
        int line = 1;
        foreach(char c in code)
        {
            if (!allowedCharacters.Contains(c))
                throw new InvalidCharacterException("Code contains invalid character '" + c + "' at line " + line);
            if (c == '\n') line++;
        }
    }

    public IPredicate ParseCode(string code)
    {
        code = RemoveSpaces(code + '\n');
        CheckForInvalidCharacters(code);
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

        List<string> variablesWithSyntaxNames = GetVariablesWithSyntaxNames();
        if(variablesWithSyntaxNames.Count > 0)
        {
            throw new InvalidNameException("The following variables have names that interfere with syntax: " + string.Join(",", variablesWithSyntaxNames));
        }

        variables.TryGetValue("return", out string? predicate);
        
        if (predicate == null)
            throw new NoReturnValueException("Code does not contain a return value");

        string finalPredicate = GetFinalPredicate(predicate);
        List<string> tokens = ShuntingYard(finalPredicate);
        if(tokens.Count > 0)
            finalPredicate = ConvertToExpression(tokens);

        return ParsePredicate(finalPredicate);

    }

    private List<string> GetVariablesWithSyntaxNames()
    {
        var flat = syntax.SelectMany(a => a).ToArray();

        List<string> variablesWithSyntaxNames = new List<string>();

        foreach(var word in flat)
        {
            if(variables.Keys.Contains(word))
            {
                variablesWithSyntaxNames.Add(word);
            }
        }
        return variablesWithSyntaxNames;
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
        pred = RemoveSpaces(pred);
        Tuple<string, List<string>> function = GetFunction(pred);
        (var functionName, _) = function;
        
        if(IsOperator(functionName))
            return ParseOperator(function);
        if(IsChessPredicate(functionName))
            return ParseChessPredicate(function);
        if(IsConstant(pred))
            return ParseConst(pred);
        
        throw new UnknownIdentifierException("Unknown identifier: " + pred);
    }

    private bool IsConstant(string pred) => pred switch
    {
        TRUE => true,
        FALSE => true,
        _ => false,
    };

    private IPredicate ParseConst(string pred)
    {
        return pred switch
        {
            TRUE => new Const(true),
            FALSE => new Const(false),
            _ => throw new ArgumentException("invalid argument"),
        };
    }

    private bool IsOperator(string word)
    {
        return word switch
        {
            OR => true,
            AND => true,
            IMPLIES => true,
            XOR => true,
            EQUALS_OPERATOR => true,
            NOT => true,
            _ => false,
        };
    }
    private bool IsChessPredicate(string word)
    {
        return word switch
        {
            movePredicate => true,
            squarePredicate => true,
            piecePredicate => true,
            countPredicate => true,
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
            OR => (arg1 | arg2),
            AND => (arg1 & arg2),
            IMPLIES => (arg1 - arg2),
            XOR => (arg1 ^ arg2),
            EQUALS_OPERATOR => new Operator(arg1, OperatorType.EQUALS, arg2),
            _ => throw new ArgumentException("Invalid operator: " + operatorType),
        };
    }


    private Comparator ParseComparator(string comparator)
    {
        return comparator switch
        {
            GREATER_THAN => Comparator.GREATER_THAN,
            LESS_THAN => Comparator.LESS_THAN,
            GREATER_THAN_OR_EQUALS => Comparator.GREATER_THAN_OR_EQUALS,
            LESS_THAN_OR_EQUALS => Comparator.LESS_THAN_OR_EQUALS,
            EQUALS_COMPARATOR => Comparator.EQUALS,
            NOT_EQUALS => Comparator.NOT_EQUALS,
            _ => throw new ArgumentException("Invalid comparator: " + comparator),
        };
    }

    private IPredicate ParseChessPredicate(Tuple<string, List<string>> predicate)
    {
        (var predType, var args) = predicate;

        return predType switch
        {
            movePredicate => ParseMovePred(args),
            piecePredicate => ParsePiecePred(args),
            squarePredicate => ParseSquarePred(args),
            countPredicate => ParseCountPred(args),
            _ => throw new ArgumentException("u done messed up"),
        };
    }
    private CountPredicate ParseCountPred(List<string> args)
    {
        string state = args[0];
        string type = args[1];
        string arg = args[2];
        string comparator = args[3];
        string compare_val = args[4];
        BoardState boardState = ParseBoardState(state);
        Comparator comparatorEnum = ParseComparator(comparator);

        return type switch
        {
            piecesLeft => new PiecesLeft(arg, comparatorEnum, int.Parse(compare_val), boardState),
            _ => throw new ArgumentException("Invalid type argument of count_pred function: " + type),
        };
    }

    private MovePredicate ParseMovePred(List<string> args)
    {
        string state = args[0];
        string type = args[1];
        string arg1 = "";
        if(type != firstMove)
            arg1 = args[2];
        string arg2 = "";
        if (type == was)
            arg2 = args[3];
        MoveState moveState = ParseMoveState(state);
        return type switch
        {
            captured => new PieceCaptured(arg1, moveState),
            pieceMoved => new PieceMoved(arg1, moveState),
            was => new MoveWas(ParsePosition(arg1), ParsePosition(arg2), moveState),
            firstMove => new FirstMove(moveState),
            _ => throw new ArgumentException("Invalid type argument of move_pred function: " + type),
        };
    }

    private PiecePredicate ParsePiecePred(List<string> args)
    {
        string state = args[0];
        string type = args[1];
        string piece = args[2];
        BoardState boardState = ParseBoardState(state);
        return type switch
        {
            attacked => new Attacked(boardState, piece),
            _ => throw new ArgumentException("Invalid type argument of move_pred function: " + type),
        };
    }

    private SquarePredicate ParseSquarePred(List<string> args)
    {
        string state = args[0];
        string position = args[1];
        string type = args[2];
        string info = "";
        if (!type.Equals("has_moved"))
            info = args[3];
        var boardState = ParseBoardState(state);
        var iPosition = ParsePosition(position);
        switch (type)
        {
            case attackedBy:
                {
                    var player = ParsePlayer(info);
                    return new SquareAttacked(iPosition, boardState, player);
                }
            case hasMoved:
                return new HasMoved(iPosition, boardState);
            case hasPiece:
               return new PieceAt(info, iPosition, boardState);
            case hasRank:
               return new SquareHasRank(iPosition, int.Parse(info));
            case hasFile:
                return new SquareHasFile(iPosition, int.Parse(info));
            case _is:
                return new SquareIs(iPosition, ParsePosition(info));
            default:
                throw new ArgumentException("Invalid type argument of square_pred function: " + type);
        };
    }

    private MoveState ParseMoveState(string state)
    {
        if (state == thisMove)
            return MoveState.THIS;
        else if (state == lastMove)
            return MoveState.LAST;
        else
            throw new ArgumentException("Invalid move_state variable: " + state);
    }


    private static RelativeTo ParseRelativeTo(string relative_to)
    {
        if (relative_to == from)
            return RelativeTo.FROM;
        else if (relative_to == to)
            return RelativeTo.TO;
        else
            throw new ArgumentException("Invalid relative_to argument: " + relative_to);
    }

    private Player ParsePlayer(string info)
    {

        if (info == black)
            return Player.Black;
        else if (info == white)
            return Player.White;
        else if(info == shared) 
            return Player.None;
        else
            throw new ArgumentException("Invalid player variable: " + info);
    }

    private static BoardState ParseBoardState(string state)
    {
        if (state == thisState)
            return BoardState.THIS;
        else if (state == nextState)
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
            case absolute:
                return new PositionAbsolute(args[0]);
            case relative:
                return new PositionRelative(int.Parse(args[1]), int.Parse(args[2]), ParseRelativeTo(args[3]));
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

    private string RemoveSpaces(string input)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (char c in input)
        {
            if (c.Equals(' ') || c.Equals('\r'))
                continue;
            else
                stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }
    private string SubstringFromTo(string str, int start, int end)
    {
        return str.Substring(start, end - start);
    }
    char[] operators = new char[] { '#', '¤', '^', '*', '+', '-'};

    private bool IsOperator(char c)
    {
        foreach(var op in operators)
        {
            if (c.Equals(op))
                return true;
        }
        return false;
    }
    private bool IsFunction(char c)
    {
        return c.Equals('!');
    }

    private bool IsIdentifier(char c)
    {
        return !IsOperator(c) && !IsFunction(c) && !c.Equals('[') && !c.Equals(']');
    }

    IDictionary<string, string> booleanOperators = new Dictionary<string, string>()
    {
        { "&&", "*"},
        {"\\|\\|", "+" },
        {"=>", "-" },
        {"==", "#" },
        {"!=", "¤" }
    };

    private string ReplaceSymbols(string input)
    {
        foreach(string op in booleanOperators.Keys)
        {
            input = Regex.Replace(input, op, booleanOperators[op]);
        }
        return input;
    }
    public List<string> ShuntingYard(string input)
    {
        input = RemoveSpaces(input);
        input = ReplaceSymbols(input);
        List<string> output = new List<string>();
        Stack<char> opStack = new Stack<char>();
        StringBuilder currentWord = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            while (IsIdentifier(input[i]))
            {
                currentWord.Append(input[i]);
                i++;
                if(i == input.Length)
                {
                    AddWord(output, currentWord);
                    break;
                }
            }
            if (i == input.Length)
            {
                break;
            }
            AddWord(output, currentWord);
            char c = input[i];
            if (IsFunction(c))
            {
                opStack.Push(c);
            }
            else if (IsOperator(c))
            {
                while (opStack.Count > 0 && HasHigherPrecedence(opStack.Peek(), c) && !opStack.Peek().Equals('('))
                {
                    output.Add(opStack.Pop().ToString());
                }
                opStack.Push(c);
            }
            else if (c.Equals('['))
            {
                opStack.Push(c);
            }
            else if (c.Equals(']'))
            {
                while (opStack.Count > 0 && !opStack.Peek().Equals('['))
                {
                    output.Add(opStack.Pop().ToString());
                }
                if (opStack.Count == 0)
                    throw new Exception("Mismatched paranthesis");
                if (opStack.Peek().Equals('['))
                    opStack.Pop();
                if (opStack.Count > 0 && IsFunction(opStack.Peek()))
                {
                    output.Add(opStack.Pop().ToString());
                }
            }
        }
        while (opStack.Count > 0)
        {
            if (opStack.Peek().Equals('['))
            {
                throw new Exception("Mismatched paranthesis");
            }
            output.Add(opStack.Pop().ToString());
        }
        return output;
    }

    public string ConvertToExpression(List<string> tokens)
    {
        Stack<string> exprStack = new Stack<string>();
        foreach (string token in tokens)
        {
            char c = token[0];
            if (IsIdentifier(c))
            {
                exprStack.Push(token);
            }
            else if (IsOperator(c))
            {
                if (exprStack.Count < 2)
                    throw new Exception($"Not enough operands for operator {token}");

                string operand2 = exprStack.Pop();
                string operand1 = exprStack.Pop();

                switch (c)
                {
                    case '*':
                        exprStack.Push($"AND({operand1},{operand2})");
                        break;
                    case '+':
                        exprStack.Push($"OR({operand1},{operand2})");
                        break;
                    case '^':
                        exprStack.Push($"XOR({operand1},{operand2})");
                        break;
                    case '#':
                        exprStack.Push($"EQUALS({operand1},{operand2})");
                        break;
                    case '¤':
                        exprStack.Push($"NOT(EQUALS({operand1},{operand2}))");
                        break;
                    case '>':
                        exprStack.Push($"IMPLIES({operand1},{operand2})");
                        break;
                    default:
                        throw new Exception($"Unknown operator: {token}");
                }
            }
            else if (token == "!")
            {
                if (exprStack.Count < 1)
                    throw new Exception($"Not enough operands for operator {token}");

                string operand = exprStack.Pop();
                exprStack.Push($"NOT({operand})");
            }
            else
            {
                throw new Exception($"Invalid token: {token}");
            }
        }

        if (exprStack.Count != 1)
            throw new Exception("Invalid expression");

        return exprStack.Pop();
    }

    private static void AddWord(List<string> output, StringBuilder currentWord)
    {
        if (currentWord.Length == 0)
            return;
        output.Add(currentWord.ToString());
        currentWord.Clear();
    }

    private bool HasHigherPrecedence(char o1, char o2)
    {
        if (IsIdentifier(o1) || IsIdentifier(o2)) throw new Exception("not operators: " + o1 + ", " + o2);
        int o1Precedence = operators.Length;
        int o2Precedence = operators.Length;
        for(int i = 0; i < operators.Length; i++)
        {
            if (operators[i].Equals(o1))
            {
                o1Precedence = i;
            }
            if (operators[i].Equals(o2))
            {
                o2Precedence = i;
            }
        }
        if (IsFunction(o1))
            o1Precedence = -1;
        if (IsFunction(o2))
            o2Precedence = -1;
        if (o1.Equals('(') || o1.Equals(')'))
            o1Precedence = -2;
        if (o2.Equals('(') || o2.Equals(')'))
            o2Precedence = -2;

        return o1Precedence <= o2Precedence;
    }
}


public class InvalidNameException : Exception
{
    public InvalidNameException(string message) : base(message)
    {

    }
}
public class NoReturnValueException : Exception
{
    public NoReturnValueException(string message) : base(message)
    {

    }
}
public class UnknownIdentifierException : Exception
{
    public UnknownIdentifierException(string message) : base(message)
    {

    }
}

public class InvalidCharacterException : Exception
{
    public InvalidCharacterException(string message) : base(message)
    {

    }
}