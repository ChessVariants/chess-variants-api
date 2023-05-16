global using Action = ChessVariantsLogic.Rules.Moves.Actions.Action;
using ChessVariantsLogic.Rules.Moves.Actions;
using ChessVariantsLogic.Rules;
using ChessVariantsLogic;
using DataAccess.MongoDB.Models;
using ChessVariantsLogic.Rules.Moves;
using ChessVariantsLogic.Rules.Predicates;

namespace ChessVariantsAPI.ObjectTranslations;

public class RuleSetTranslator
{
    public static RuleSet? ConstructFromModel(ISet<Tuple<MoveTemplateModel, string>> moveTemplateModels, List<Tuple<EventModel, string>> eventModels, List<Tuple<EventModel, string>> stalemateModels, string predicateScript)
    {
        ISet<MoveTemplate?> moveTemplates = moveTemplateModels.Select(tuple => ConstructMoveTemplateFromModel(tuple.Item1, tuple.Item2)).ToHashSet();
        if (moveTemplates.Any(item => item == null)) return null;

        ISet<Event?> events = eventModels.Select(tuple => ConstructEventFromModel(tuple.Item1, tuple.Item2)).ToHashSet();
        if (events.Any(item => item == null)) return null;

        ISet<Event?> stalemateEvents = stalemateModels.Select(tuple => ConstructEventFromModel(tuple.Item1, tuple.Item2)).ToHashSet();
        if (stalemateEvents.Any(item => item == null)) return null;

        IPredicate predicate;
        try
        {
            predicate = PredicateParser.ParseCode(predicateScript);
        }
        catch (Exception)
        {
            return null;
        }
        return new RuleSet(predicate, moveTemplates as ISet<MoveTemplate>, events as ISet<Event>, stalemateEvents as ISet<Event>);
    }

    public static Event? ConstructEventFromModel(EventModel eventModel, string predicateScript)
    {
        List<Action> actions = new();
        foreach (var actionModel in eventModel.Actions)
        {
            var action = ConstructActionFromModel(actionModel);
            if (action == null) return null;
            actions.Add(action);
        }
        IPredicate predicate;
        try
        {
            predicate = PredicateParser.ParseCode(predicateScript);
        }
        catch (Exception)
        {
            return null;
        }
        return new Event(predicate, actions);
    }

    public static MoveTemplate? ConstructMoveTemplateFromModel(MoveTemplateModel moveTemplateModel, string predicateScript)
    {
        List<Action> actions = new();
        foreach (var actionModel in moveTemplateModel.Actions)
        {
            Action? action = ConstructActionFromModel(actionModel);
            if (action == null) return null;
            actions.Add(action);
        }
        string identifier = moveTemplateModel.Identifier;
        IPosition? position = ConstructPositionFromModel(moveTemplateModel.Click);
        if (position == null) return null;
        IPredicate predicate;
        try
        {
            predicate = PredicateParser.ParseCode(predicateScript);
        }
        catch (Exception)
        {
            return null;
        }
        return new MoveTemplate(actions, predicate, identifier, position);
    }

    public static Action? ConstructActionFromModel(ActionRec actionRec)
    {
        if (actionRec.Move != null)
        {
            var from = ConstructPositionFromModel(actionRec.Move.From);
            var to = ConstructPositionFromModel(actionRec.Move.To);
            if (from == null || to == null)
                return null;
            return new ActionMovePiece(from, to);
        }
        else if (actionRec.Set != null)
        {
            var at = ConstructPositionFromModel(actionRec.Set.At);
            if (at == null)
                return null;
            return new ActionSetPiece(at, actionRec.Set.Identifier);
        }
        else if (actionRec.Win != null)
        {
            return new ActionWin(actionRec.Win.WhiteWins ? Player.White : Player.Black);
        }
        else if (actionRec.IsTie)
        {
            return new ActionTie();
        }
        else if (actionRec.IsPromotion)
        {
            return new ActionPromotionTime();
        }
        return null;
    }

    public static IPosition? ConstructPositionFromModel(Position positionRec)
    {
        if (positionRec.PositionAbsolute != null)
        {
            return new PositionAbsolute(positionRec.PositionAbsolute.Coordinate);
        }
        else if (positionRec.PositionRelative != null)
        {
            return new PositionRelative(positionRec.PositionRelative.Y, positionRec.PositionRelative.X, positionRec.PositionRelative.To ? RelativeTo.TO : RelativeTo.FROM);
        }
        return null;
    }

}
