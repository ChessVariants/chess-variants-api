using ChessVariantsLogic.Rules.Predicates.ChessPredicates;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessVariantsLogic.Rules.Predicates;

/// <summary>
/// A parser for translating a simple DSL to an IPredicate object.
/// </summary>
public class PredicateParser
{


    /*
        DOCUMENTATION OF THE PREDICATE DSL

        This documentation is probably not very understandable at the moment and is mostly for my own use.
        If you have any questions, feel free to ask me //Joakim
      
        operators: &,    |,      =>,   ^,     ==,         !=,   !
        operators: AND, OR, IMPLIES, XOR, EQUALS, NOT_EQUALS, NOT

        paranthesis for boolean precedence: []

        VARIABLE TYPES

        BoolPredicate : {true, false}

        RelativeTo : {from, to}

        Integer : {1, 2, 3, 4, ... n}

        Coord : {a1, a2, a3, ... t20}

        Color : {white, black, shared}

        BoardState : {this_state, next_state}

        MoveState : {this_move, last_move}

        PieceName : {ki, bi, ... PA}

        Square : {absolute(Coord coordinate), relative(int x, int y, RelativeTo relativeTo)}

        Comparator : {greater_than, less_than, greater_than_or_equals, less_than_or_equals, equals, not_equals}

        PREDICATES:

        COUNT PREDICATES:
        count_pieces_with_id(BoardState boardState, PieceIdentifier pieceIdentifier, Comparator comparator, Integer compare_val)

        PIECE PREDICATES:

        piece_attacked(BoardState state, PieceIdentifier pieceIdentifier)

        MOVE PREDICATES:

        move_captured	(MoveState state, PieceIdentifier pieceIdentifier)
        move_piece_is	(MoveState state, PieceIdentifier pieceIdentifier)
        move_from_to	(MoveState state, Square from, Square to)
        move_first		(MoveState state)

        SQUARE PREDICATES:

        square_attacked	(BoardState boardState, Square square, Color attackedBy)
        square_has_piece	(BoardState boardState, Square square, PieceIdentifier pieceIdentifier)
        square_is	 	(Square square, Square square)
        square_has_rank	(Square square, int rank)
        square_has_file	(Square square, char file)
        square_piece_has_moved  (BoardState boardState, Square square)

*/

    #region BoardStates
    private const string THIS_STATE = "this_state";
    private const string NEXT_STATE = "next_state";
    private static readonly string[] boardstates = new string[] { THIS_STATE, NEXT_STATE };
    #endregion

    #region Comparators
    private const string GREATER_THAN = "greater_than";
    private const string LESS_THAN = "less_than";
    private const string GREATER_THAN_OR_EQUALS = "greater_than_or_equals";
    private const string LESS_THAN_OR_EQUALS = "less_than_or_equals";
    private const string EQUALS_COMPARATOR = "equals";
    private const string NOT_EQUALS = "not_equals";
    private static readonly string[] comparators = new string[] { GREATER_THAN, LESS_THAN, GREATER_THAN_OR_EQUALS, LESS_THAN_OR_EQUALS, EQUALS_COMPARATOR, NOT_EQUALS };
    #endregion

    #region ConstValues
    private const string TRUE = "true";
    private const string FALSE = "false";
    private static readonly string[] constValues = new string[] { TRUE, FALSE };
    #endregion

    #region CountPredicateTypes
    private const string piecesLeft = "count_pieces_with_id";
    private static readonly string[] countPredicateTypes = new string[] { piecesLeft };
    #endregion

    #region MovePredicateTypes
    private const string CAPTURED = "move_captured";
    private const string PIECE_MOVED = "move_piece_is";
    private const string WAS = "move_from_to";
    private const string FIRST_MOVE = "move_first";
    private static readonly string[] movePredicateTypes = new string[] { CAPTURED, PIECE_MOVED, WAS, FIRST_MOVE };
    #endregion

    #region MoveStates
    private const string THIS_MOVE = "this_move";
    private const string LAST_MOVE = "last_move";
    private static readonly string[] moveStates = new string[] { THIS_MOVE, LAST_MOVE };
    #endregion

    #region Operators

    private const string AND_OP = "&&";
    private const string OR_OP =  "\\|\\|";
    private const string IMPLIES_OP = "=>";
    private const string EQUALS_OP = "==";
    private const string NOT_EQUALS_OP = "!=";
    private const string XOR_OP = "^";

    private const string AND = "AND";
    private const string OR = "OR";
    private const string IMPLIES = "IMPLIES";
    private const string XOR = "XOR";
    private const string EQUALS_OPERATOR = "EQUALS";
    private const string NOT = "NOT";
    private static readonly string[] operatorTypes = new string[] { AND, OR, IMPLIES, XOR, EQUALS_OPERATOR, NOT, AND_OP, OR_OP, IMPLIES_OP, EQUALS_OP, NOT_EQUALS_OP, XOR_OP };

    #endregion

    #region PieceClassifiers
    private const string WHITE = "WHITE";
    private const string BLACK = "BLACK";
    private const string SHARED = "SHARED";
    private static readonly string[] pieceClassifiers = new string[] { WHITE, BLACK, SHARED };
    #endregion

    #region PiecePredicateTypes
    private const string ATTACKED = "piece_attacked";
    private static readonly string[] piecePredicateTypes = new string[] { ATTACKED };
    #endregion

    #region Predicates
    private const string COUNT_PREDICATE = "count";
    private const string MOVE_PREDICATE = "move";
    private const string PIECE_PREDICATE = "piece";
    private const string SQUARE_PREDICATE = "square";
    private static readonly string[] predicateTypes = new string[] { COUNT_PREDICATE, MOVE_PREDICATE, PIECE_PREDICATE, SQUARE_PREDICATE };
    #endregion

    #region RelativeToTypes
    private const string FROM = "from";
    private const string TO = "to";
    private static readonly string[] relativeTo = new string[] { FROM, TO };
    #endregion

    #region SquarePredicateTypes
    private const string SQUARE_ATTACKED = "square_attacked";
    private const string SQUARE_HAS_MOVED = "square_has_moved";
    private const string SQUARE_IS = "square_is";
    private const string SQUARE_HAS_PIECE = "square_has_piece";
    private const string SQUARE_HAS_RANK = "square_has_rank";
    private const string SQUARE_HAS_FILE = "square_has_file";
    private static readonly string[] squarePredicateTypes = new string[] { SQUARE_ATTACKED, SQUARE_HAS_MOVED, SQUARE_IS, SQUARE_HAS_PIECE, SQUARE_HAS_RANK, SQUARE_HAS_FILE };
    #endregion

    #region SquareTypes
    private const string ABSOLUTE = "absolute";
    private const string RELATIVE = "relative";
    private static readonly string[] squareTypes = new string[] { ABSOLUTE, RELATIVE };
    #endregion




    private static string[][] syntax = new string[][] { predicateTypes, operatorTypes, constValues, countPredicateTypes, comparators, movePredicateTypes, piecePredicateTypes, squarePredicateTypes, relativeTo, squareTypes, pieceClassifiers, boardstates, moveStates };

    private static string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHJIKLMNOPQRSTUVWXYZ0123456789-;(),[]&|!=>_^\n";

    private static void CheckForInvalidCharacters(string code)
    {
        int line = 1;
        foreach(char c in code)
        {
            if (!allowedCharacters.Contains(c))
                throw new PrediChessException("Code contains invalid character '" + c + "' at line " + line);
            if (c == '\n') line++;
        }
    }

    public static IPredicate ParseCode(string code)
    {
        IDictionary<string, string> variables = new Dictionary<string, string>();
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

        List<string> variablesWithSyntaxNames = GetVariablesWithSyntaxNames(variables);
        if(variablesWithSyntaxNames.Count > 0)
        {
            throw new PrediChessException("The following variables have names that interfere with syntax: " + string.Join(",", variablesWithSyntaxNames));
        }

        variables.TryGetValue("return", out string? predicate);
        
        if (predicate == null)
            throw new PrediChessException("Code does not contain a return value");

        string finalPredicate = GetFinalPredicate(predicate, variables);
        List<string> tokens = ShuntingYard(finalPredicate);
        if(tokens.Count > 0)
            finalPredicate = ConvertToExpression(tokens);

        return ParsePredicate(finalPredicate);

    }

    private static List<string> GetVariablesWithSyntaxNames(IDictionary<string,string> variables)
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

    private static string ReplaceWord(string input, string wordToFind, string replace)
    {
        string pattern = string.Format(@"\b{0}\b", wordToFind);
        return Regex.Replace(input, wordToFind, replace);
    }



    private static string GetFinalPredicate(string predicate, IDictionary<string, string> variables)
    {
        while (ContainsAnyVariable(predicate, variables))
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

    private static bool ContainsAnyVariable(string predicate, IDictionary<string, string> variables)
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

    public static IPredicate ParsePredicate(string pred)
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
        
        throw new PrediChessException("Unknown identifier: " + pred);
    }

    private static bool IsConstant(string pred) => pred switch
    {
        TRUE => true,
        FALSE => true,
        _ => false,
    };

    private static IPredicate ParseConst(string pred)
    {
        return pred switch
        {
            TRUE => new Const(true),
            FALSE => new Const(false),
            _ => throw new PrediChessException("invalid argument"),
        };
    }

    private static bool IsOperator(string word)
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
    private static bool IsChessPredicate(string word)
    {
        if (word.StartsWith(COUNT_PREDICATE))
        {
            return true;
        }
        else if (word.StartsWith(PIECE_PREDICATE))
        {
            return true;
        }
        else if (movePredicateTypes.Contains(word))
        {
            return true;
        }
        else if (squarePredicateTypes.Contains(word))
        {
            return true;
        }
        return false;
    }


    private static IPredicate ParseOperator(Tuple<string, List<string>> operatorFunction)
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
            _ => throw new PrediChessException("Invalid operator: " + operatorType),
        };
    }


    private static Comparator ParseComparator(string comparator)
    {
        return comparator switch
        {
            GREATER_THAN => Comparator.GREATER_THAN,
            LESS_THAN => Comparator.LESS_THAN,
            GREATER_THAN_OR_EQUALS => Comparator.GREATER_THAN_OR_EQUALS,
            LESS_THAN_OR_EQUALS => Comparator.LESS_THAN_OR_EQUALS,
            EQUALS_COMPARATOR => Comparator.EQUALS,
            NOT_EQUALS => Comparator.NOT_EQUALS,
            _ => throw new PrediChessException("Invalid comparator: " + comparator),
        };
    }

    private static IPredicate ParseChessPredicate(Tuple<string, List<string>> predicate)
    {
        var predName = predicate.Item1;
        string predType = GetPredType(predName);

        return predType switch
        {
            COUNT_PREDICATE => ParseCountPred(predicate),
            PIECE_PREDICATE => ParsePiecePred(predicate),
            MOVE_PREDICATE => ParseMovePred(predicate),
            SQUARE_PREDICATE => ParseSquarePred(predicate),
            _ => throw new PrediChessException("Unknown identifier: " + predName),
        };
    }

    private static string GetPredType(string predName)
    {
        if(predName.StartsWith(COUNT_PREDICATE))
        {
            return COUNT_PREDICATE;
        }
        else if(predName.StartsWith(PIECE_PREDICATE))
        {
            return PIECE_PREDICATE;
        }
        else if(predName.StartsWith(MOVE_PREDICATE))
        {
            return MOVE_PREDICATE;
        }
        else if (squarePredicateTypes.Contains(predName))
        {
            return SQUARE_PREDICATE;
        }
        throw new PrediChessException("Unknown identifier: " + predName);
    }


    private static CountPredicate ParseCountPred(Tuple<string, List<string>> predicate)
    {
        var (type, args) = predicate;
        string state = args[0];
        string arg = args[1];
        string comparator = args[2];
        string compareVal = args[3];
        BoardState boardState = ParseBoardState(state);
        Comparator comparatorEnum = ParseComparator(comparator);

        int val = int.Parse(compareVal);


        return type switch
        {
            piecesLeft => new PiecesLeft(arg, comparatorEnum, val, boardState),
            _ => throw new PrediChessException("Invalid type argument of count_pred function: " + type),
        };
    }

    private static MovePredicate ParseMovePred(Tuple<string, List<string>> predicate)
    {
        var (type, args) = predicate;
        string state = args[0];
        string arg1 = "";
        if(type != FIRST_MOVE)
            arg1 = args[1];
        string arg2 = "";
        if (type == WAS)
            arg2 = args[2];
        MoveState moveState = ParseMoveState(state);
        return type switch
        {
            CAPTURED => new PieceCaptured(arg1, moveState),
            PIECE_MOVED => new PieceMoved(arg1, moveState),
            WAS => new MoveWas(ParsePosition(arg1), ParsePosition(arg2), moveState),
            FIRST_MOVE => new FirstMove(moveState),
            _ => throw new PrediChessException("Invalid type argument of move_pred function: " + type),
        };
    }

    private static PiecePredicate ParsePiecePred(Tuple<string, List<string>> predicate)
    {
        var (type, args) = predicate;
        string state = args[0];
        string piece = args[1];
        BoardState boardState = ParseBoardState(state);
        return type switch
        {
            ATTACKED => new Attacked(boardState, piece),
            _ => throw new PrediChessException("Invalid type argument of move_pred function: " + type),
        };
    }

    private static SquarePredicate ParseSquarePred(Tuple<string, List<string>> predicate)
    {
        var (type, args) = predicate;
        string arg0 = args[0];
        string arg1 = args[1];
        string arg2 = "";
        if (type != SQUARE_HAS_MOVED && type != SQUARE_IS && type != SQUARE_HAS_RANK && type != SQUARE_HAS_FILE)
            arg2 = args[2];
        
        return type switch
        {
            SQUARE_ATTACKED => new SquareAttacked(ParsePosition(arg1), ParseBoardState(arg0), ParsePlayer(arg2)),
            SQUARE_HAS_PIECE => new PieceAt(arg2, ParsePosition(arg1), ParseBoardState(arg0)),
            SQUARE_HAS_MOVED => new HasMoved(ParsePosition(arg1), ParseBoardState(arg0)),
            SQUARE_IS => new SquareIs(ParsePosition(arg0), ParsePosition(arg1)),
            SQUARE_HAS_RANK => new SquareHasRank(ParsePosition(arg0), int.Parse(arg1)),
            SQUARE_HAS_FILE => new SquareHasFile(ParsePosition(arg0), int.Parse(arg1)),
            _ => throw new PrediChessException("Unknown Identifier: " + type),
        };
        ;
    }

    private static MoveState ParseMoveState(string state)
    {
        if (state == THIS_MOVE)
            return MoveState.THIS;
        else if (state == LAST_MOVE)
            return MoveState.LAST;
        else
            throw new PrediChessException("Invalid move_state variable: " + state);
    }


    private static RelativeTo ParseRelativeTo(string relative_to)
    {
        if (relative_to == FROM)
            return RelativeTo.FROM;
        else if (relative_to == TO)
            return RelativeTo.TO;
        else
            throw new PrediChessException("Invalid relative_to argument: " + relative_to);
    }

    private static Player ParsePlayer(string info)
    {
        if (info == BLACK)
            return Player.Black;
        else if (info == WHITE)
            return Player.White;
        else if(info == SHARED) 
            return Player.None;
        else
            throw new PrediChessException("Invalid player variable: " + info);
    }

    private static BoardState ParseBoardState(string state)
    {
        if (state == THIS_STATE)
            return BoardState.THIS;
        else if (state == NEXT_STATE)
            return BoardState.NEXT;
        else
            throw new PrediChessException("Invalid state variable: " + state);
    }

    private static IPosition ParsePosition(string position)
    {
        var func = GetFunction(position);
        (var functionName, var args) = func;
        switch(functionName)
        {
            case ABSOLUTE:
                return new PositionAbsolute(args[0]);
            case RELATIVE:
                return new PositionRelative(int.Parse(args[0]), int.Parse(args[1]), ParseRelativeTo(args[2]));
            default:
                throw new PrediChessException("Invalid position: " + position);
        }

    }

    private static Tuple<string, List<string>> GetFunction(string function)
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

        throw new PrediChessException("function " + function + "  isn't a proper function");
    }
    private static string GetFirstWord(string line)
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

    private static string RemoveSpaces(string input)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (char c in input)
        {
            if (c.Equals(' ') || c.Equals('\r') || c.Equals('\t'))
                continue;
            else
                stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }
    private static string SubstringFromTo(string str, int start, int end)
    {
        return str.Substring(start, end - start);
    }

    // Yes this is an ugly fix because I couldn't figure out how to make the shunting yard algorithm work with operators with several symbols such as '=>' or '&&' (as opposed to using a single symbol such as '!').
    // Instead they get translated to a single symbol last minute.

    private static readonly IDictionary<string, string> _booleanOperators = new Dictionary<string, string>()
    {
        {AND_OP, "*"},
        {OR_OP, "+" },
        {IMPLIES_OP, "%" },
        {EQUALS_OP, "#" },
        {NOT_EQUALS_OP, "¤" }
    };

    private readonly static char[] operators = new char[] { '#', '¤', '^', '*', '+', '%'};

    private static bool IsOperator(char c)
    {
        foreach(var op in operators)
        {
            if (c.Equals(op))
                return true;
        }
        return false;
    }
    private static bool IsFunction(char c)
    {
        return c.Equals('!');
    }

    private static bool IsIdentifier(char c)
    {
        return !IsOperator(c) && !IsFunction(c) && !c.Equals('[') && !c.Equals(']');
    }


    private static string ReplaceSymbols(string input)
    {
        foreach(string op in _booleanOperators.Keys)
        {
            input = Regex.Replace(input, op, _booleanOperators[op]);
        }
        return input;
    }
    public static List<string> ShuntingYard(string input)
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
                    throw new PrediChessException("Mismatched paranthesis");
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
                throw new PrediChessException("Mismatched paranthesis");
            }
            output.Add(opStack.Pop().ToString());
        }
        return output;
    }

    public static string ConvertToExpression(List<string> tokens)
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
                    throw new PrediChessException($"Not enough operands for operator {token}");

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
                        throw new PrediChessException($"Unknown operator: {token}");
                }
            }
            else if (token == "!")
            {
                if (exprStack.Count < 1)
                    throw new PrediChessException($"Not enough operands for operator {token}");

                string operand = exprStack.Pop();
                exprStack.Push($"NOT({operand})");
            }
            else
            {
                throw new PrediChessException($"Invalid token: {token}");
            }
        }

        if (exprStack.Count != 1)
            throw new PrediChessException("Invalid expression");

        return exprStack.Pop();
    }

    private static void AddWord(List<string> output, StringBuilder currentWord)
    {
        if (currentWord.Length == 0)
            return;
        output.Add(currentWord.ToString());
        currentWord.Clear();
    }

    private static bool HasHigherPrecedence(char o1, char o2)
    {
        if (IsIdentifier(o1) || IsIdentifier(o2)) throw new PrediChessException("not operators: " + o1 + ", " + o2);
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

public class PrediChessException : Exception
{
    public PrediChessException(string message) : base(message)
    {

    }
}
