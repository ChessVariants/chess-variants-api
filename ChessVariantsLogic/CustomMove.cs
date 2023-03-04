namespace ChessVariantsLogic;

using Predicates;

public class CustomMove
{
    private IEnumerable<IAction> _actions;
    private IPredicate _predicate;
    private string _from;
    private string _to;

    public CustomMove(IEnumerable<IAction> actions, IPredicate predicate, string from, string to)
    {
        _actions = actions;
        _predicate = predicate;
        _from = from;
        _to = to;


    }
    public IEnumerable<IAction> Actions { get { return _actions; } }
    public IPredicate Predicate { get { return _predicate; } }
    public string From { get { return _from;} }
    public string To { get { return _to;} }

}