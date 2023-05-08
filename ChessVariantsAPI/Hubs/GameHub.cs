using ChessVariantsAPI.GameOrganization;
using ChessVariantsAPI.Hubs.DTOs;
using ChessVariantsLogic;
using ChessVariantsLogic.Export;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ChessVariantsAPI.ObjectTranslations;
using DataAccess.MongoDB.Models;
using ChessVariantsLogic.Rules;
using System.Linq;
using DataAccess.MongoDB.Repositories;

namespace ChessVariantsAPI.Hubs;

/// <summary>
/// A SignalR hub for handling real-time chess game communication between client and server.
/// </summary>
[Authorize]
public class GameHub : Hub
{
    private readonly GameOrganizer _organizer;
    private readonly GroupOrganizer _groupOrganizer;
    readonly protected ILogger _logger;
    private DatabaseService _db;


    public GameHub(GameOrganizer organizer, ILogger<GameHub> logger, GroupOrganizer groupOrganizer, DatabaseService db)
    {
        _organizer = organizer;
        _groupOrganizer = groupOrganizer;
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Adds the caller to a group corresponding to the supplied <paramref name="gameId"/>.
    /// </summary>
    /// <param name="gameId">The id for the game to join</param>
    /// <returns></returns>
    [Authorize]
    public async Task<JoinResultDTO> JoinGame(string gameId)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to join game with id <{gameid}>", user, gameId);

            if (!_organizer.PlayerAbleToJoin(gameId, user))
            {
                return new JoinResultDTO { Color = null, Success = false, FailReason = "The game you tried to join is already full" };
            }

            Player player;
            if (_groupOrganizer.InGroup(user, gameId))
            {
                await AddToGroup(user, gameId);
                player = _organizer.GetPlayer(gameId, user);
            }
            else
            {
                await AddToGroup(user, gameId);
                player = _organizer.JoinGame(gameId, user);
                await Clients.Caller.SendGameJoined(player.AsString(), user);
                await Clients.Groups(gameId).SendPlayerJoinedGame(player.AsString(), user);
            }

            _logger.LogDebug("User <{user}> joined game <{gameid}>", user, gameId);
            return new JoinResultDTO { Color = player.AsString(), Success = true };
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to join game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
            return new JoinResultDTO { Success = false, FailReason = e.Message };
        }
    }

    public async Task<SetVariantDTO> SetGame(string gameId, string variantIdentifier)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to set game-variant {variant} for game with id <{gameid}>", user, variantIdentifier, gameId);
            var result = _organizer.SetGame(gameId, variantIdentifier);
            if (result == false)
            {
                await Clients.Caller.SendGenericError($"Could not set game variant to {variantIdentifier}");
                return new SetVariantDTO { Success = false, FailReason = $"Could not set game variant to {variantIdentifier}" };
            }
            await Clients.Groups(gameId).SendGameVariantSet(variantIdentifier);
            _logger.LogDebug("User <{user}> set game variant to {variant} for game <{gameid}> result: {bool}", user, variantIdentifier, gameId, result);
            return new SetVariantDTO { Success = true };
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to create game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
            return new SetVariantDTO { Success = false, FailReason = e.Message };
        }
    }

    public async Task StartGame(string gameId)
    {
        try
        {
            var user = GetUsername();
            var success = _organizer.StartGame(gameId, user);

            if (!success)
            {
                _logger.LogInformation("Tried to start game {g} but could not", gameId);
                return;
            }
            await Clients.Groups(gameId).SendGameStarted(_organizer.GetColorsObject(gameId));
            var results = _organizer.MakePendingAIMoveIfApplicable(gameId);
            foreach (var result in results)
            {
                await CommunicateGameEvents(gameId, result, "AIMove", Clients.Caller);
            }
            var state = _organizer.GetState(gameId);
            await Clients.Groups(gameId).SendUpdatedGameState(state);
            _logger.LogInformation("Started game {g}", gameId);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to start game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
        }
    }

    /// <summary>
    /// Adds the caller to a group corresponding to the supplied <paramref name="gameId"/> and creates the game.
    /// </summary>
    /// <param name="gameId">The id for the game to join</param>
    /// <param name="variantIdentifier">What variant to create</param>
    /// <returns></returns
    public async Task<SetVariantDTO> CreateGame(string gameId, string variantIdentifier)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to create game with id <{gameid}>", user, gameId);
            Player createdPlayer;

            var type = _organizer.GetVariantType(gameId);
            if (type == VariantType.Predefined)
            {
                createdPlayer = _organizer.CreateGame(gameId, user, variantIdentifier);
            } else
            {
                var variantList = await _db.Variants.FindAsync(v => v.Creator == user && v.Code == variantIdentifier);
                var variantToPlay = variantList.First();

                var whiteRulesetModelList = await _db.RuleSets.FindAsync(r => r.Name == variantToPlay.WhiteRuleSetIdentifier && r.CreatorName == user);
                var blackRulesetModelList = await _db.RuleSets.FindAsync(r => r.Name == variantToPlay.BlackRuleSetIdentifier && r.CreatorName == user);
                var boardModelList = await _db.Chessboards.FindAsync(b => b.Name == variantToPlay.BoardIdentifier && b.Creator == user);

                var whiteRulesetModel = whiteRulesetModelList.First();
                var blackRulesetModel = blackRulesetModelList.First();

                var boardModel = boardModelList.First();

                var whiteRuleset = await CreateRuleSet(user, whiteRulesetModel);
                var blackRuleset = await CreateRuleSet(user, blackRulesetModel);

                List<ChessVariantsLogic.Piece> pieces = new();
                var pieceModelList = await _db.Pieces.FindAsync(p => boardModel.Board.Contains(p.Name) && (p.Creator == user || p.Creator == "admin"));
                var pieceLogicSet = pieceModelList.Select(pm => PieceTranslator.CreatePieceLogic(pm)).ToHashSet();
                createdPlayer = _organizer.CreateCustomGame(gameId, user, variantIdentifier, whiteRuleset, blackRuleset, variantToPlay.MovesPerTurn, pieceLogicSet, boardModel.Rows, boardModel.Cols, boardModel.Board);
            }


            await AddToGroup(user, gameId);
            await Clients.Caller.SendGameCreated(createdPlayer.AsString(), user);
            return new SetVariantDTO { Success = true };
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to create game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
            return new SetVariantDTO { Success = false, FailReason = e.Message };
        }
    }

    private async Task<RuleSet> CreateRuleSet(string user, RuleSetModel rulesetModel)
    {
        var eventModels = await CreateEventModelTuples(user, rulesetModel.Events, _db.Events);
        var stalemateEventModels = await CreateEventModelTuples(user, rulesetModel.StalemateEvents, _db.Events);

        var movetemplateList = await _db.Moves.FindAsync(mt => rulesetModel.Moves.Contains(mt.Name) && mt.CreatorName == user);
        var movetemplatePredicateList = await _db.Predicates.FindAsync(p => movetemplateList.Any(mt => mt.Predicate == p.Name) && p.CreatorName == user);
        movetemplateList = movetemplateList.OrderBy(mt => mt.Name).ToList();
        movetemplatePredicateList = movetemplatePredicateList.OrderBy(p => p.Name).ToList();
        var movetemplateModels = movetemplateList.Zip(movetemplatePredicateList, (mt, p) => Tuple.Create(mt, p.Code)).ToList();

        HashSet<Tuple<MoveTemplateModel, string>> moveTemplateModels = new();
        foreach (var moveTemplateIdentifier in rulesetModel.Moves)
        {
            var mtmList = await _db.Moves.FindAsync(mt => mt.Name == moveTemplateIdentifier && mt.CreatorName == user);
            var mtm = mtmList.First();
            var predicateList = await _db.Predicates.FindAsync(p => p.Name == mtm.Predicate && p.CreatorName == user);
            var predicate = predicateList.First();
            moveTemplateModels.Add(Tuple.Create(mtm, predicate.Code));
        }

        var movePredicateList = await _db.Predicates.FindAsync(p => p.Name == rulesetModel.Predicate && p.CreatorName == user);
        var movePredicate = movePredicateList.First();

        var ruleset = RuleSetTranslator.ConstructFromModel(moveTemplateModels, eventModels, stalemateEventModels, movePredicate.Code);
        return ruleset!;
    }

    private async Task<List<Tuple<EventModel, string>>> CreateEventModelTuples(string user, List<string> events, EventRepository eventRepo)
    {
        var eventModelList = await eventRepo.FindAsync(e => events.Contains(e.Name) && e.CreatorName == user);
        var eventModelPredicateList = await _db.Predicates.FindAsync(p => eventModelList.Any(e => e.Predicate == p.Name) && p.CreatorName == user);
        eventModelList = eventModelList.OrderBy(e => e.Name).ToList();
        eventModelPredicateList = eventModelPredicateList.OrderBy(p => p.Name).ToList();
        var eventModels = eventModelList.Zip(eventModelPredicateList, (e, p) => Tuple.Create(e, p.Code)).ToList();
        return eventModels!;
    }

    /// <summary>
    /// Removes the caller from a group corresponding to the supplied <paramref name="gameId"/>. Deletes the game if its empty.
    /// </summary>
    /// <param name="gameId">The id for the game to leave</param>
    /// <returns></returns>
    public async Task LeaveGame(string gameId)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to create game with id <{gameid}>", user, gameId);
            await RemoveFromGroup(user, gameId);
            await Clients.Caller.SendGameLeft();
            bool playerLeft = _organizer.LeaveGame(gameId, user);
            if (playerLeft)
            {
                await Clients.Groups(gameId).SendPlayerLeftGame(user);
            }
        }
        catch (GameEmptyException)
        {
            _organizer.DeleteGame(gameId);
            _logger.LogDebug("Game with id <{gameid}> was deleted", gameId);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to leave game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
        }
    }

    public async Task AssignAI(string gameId)
    {
        try
        {
            var color = _organizer.AssignAI(gameId);
            await Clients.Caller.SendPlayerJoinedGame(color.AsString(), "AI");
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to add an AI to game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
        }
    }

    /// <summary>
    /// Requests a swap of colors
    /// </summary>
    /// <param name="gameId">What game to swap colors in</param>
    /// <returns></returns>
    public async Task SwapColors(string gameId)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to swap colors in id <{gameid}>", user, gameId);
            _organizer.SwapColors(gameId, user);
            await Clients.Group(gameId).SendColors(_organizer.GetColorsObject(gameId));
            _logger.LogDebug("Colors swapped in game <{gameid}>", gameId);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to swap colors in game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }
    }

    public ColorsDTO? RequestColors(string gameId)
    {
        try
        {
            var user = GetUsername();
            var colors = _organizer.GetColorsObject(gameId);
            _logger.LogDebug("User <{user}> requested colors in game <{gameid}>", user, gameId);
            return colors;
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When requesting colors in game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            return null;
        }
    }

    public async Task PromotePiece(string gameId, string pieceIdentifier)
    {
        try
        {
            var user = GetUsername();
            var result = _organizer.PromotePiece(gameId, pieceIdentifier);
            _logger.LogDebug("User <{user}> trying to promote a {pieceIdentifier} in <{gameid}>", user, pieceIdentifier, gameId);
            await Clients.Caller.SendPromotionDone();
            await CommunicateGameEvents(gameId, result, "promotionMove", Clients.Caller);
            var results = _organizer.MakePendingAIMoveIfApplicable(gameId);
            foreach (var AIResult in results)
            {
                await CommunicateGameEvents(gameId, AIResult, "AIMove", Clients.Caller);
            }
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendGenericError(e.Message);
        }
    }

    /// <summary>
    /// Makes a move on the board if the move is valid and informs users of the gamestate.
    /// </summary>
    /// <param name="move">Move requested to be made</param>
    /// <param name="gameId">Id of the game</param>
    /// <returns></returns>
    public async Task MovePiece(string move, string gameId)
    {
        // if move is valid, compute new board
        //ISet<GameEvent>? result;
        //GameState? state;
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to make move <{move}> in game <{gameid}>", user, move, gameId);
            var results = _organizer.Move(move, gameId, user);

            foreach (var result in results)
            {
                if (result.Contains(GameEvent.Promotion))
                {
                    var promotablePieces = _organizer.GetPromotablePieces(gameId);
                    var pieceInfos = promotablePieces.Select(p => p.PieceIdentifier).ToList();
                    var currentPlayer = _organizer.GetPlayer(gameId, user).AsString();
                    var promotionOptionsDTO = new PromotionOptionsDTO { PromotablePieces = pieceInfos, Player = currentPlayer};
                    _logger.LogDebug("User <{user}> in <{gameid}> ready to make a promotion with pieces: {options}", user, gameId, pieceInfos);
                    await Clients.Caller.SendUpdatedGameState(_organizer.GetState(gameId));
                    await Clients.Caller.SendPromotionOptions(promotionOptionsDTO);
                    return;
                }

                await CommunicateGameEvents(gameId, result, move, Clients.Caller);
            }
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendGenericError(e.Message);
            return;
        }
    }

    private async Task CommunicateGameEvents(string gameId, ISet<GameEvent> result, string move, IClientProxy caller)
    {
        if (result.Contains(GameEvent.InvalidMove))
        {
            _logger.LogDebug("Move <{move}> in game <{gameid}> was invalid", move, gameId);
            await caller.SendInvalidMove();
            return;
        }
        if (result.Contains(GameEvent.MoveSucceeded))
        {
            _logger.LogDebug("Move <{move}> in game <{gameid}> was successful", move, gameId);
            var state = _organizer.GetState(gameId);
            await Clients.Groups(gameId).SendUpdatedGameState(state!);
        }
        if (result.Contains(GameEvent.WhiteWon))
        {
            _logger.LogDebug("Move <{move}> in game <{gameid}> won the game for white", move, gameId);
            await Clients.Group(gameId).SendWhiteWon();
        }
        else if (result.Contains(GameEvent.BlackWon))
        {
            _logger.LogDebug("Move <{move}> in game <{gameid}> won the game for black", move, gameId);
            await Clients.Group(gameId).SendBlackWon();
        }
        else if (result.Contains(GameEvent.Tie))
        {
            _logger.LogDebug("Move <{move}> in game <{gameid}> resulted in a tie", move, gameId);
            await Clients.Group(gameId).SendTie();
        }
    }

    /// <summary>
    /// Responds to the caller with the current game state
    /// </summary>
    /// <param name="gameId">The game to request state for</param>
    /// <returns></returns>
    public GameState? RequestState(string gameId)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("Board state for game <{gameid}> was requested by {u}", gameId, user);
            if (!_organizer.PlayerInGame(gameId, user)) return null;
            return _organizer.GetState(gameId);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When requesting board state for game <{gameid}> the following error occured: {e}", gameId, e.Message);
            return null;
        }
    }

    private string GetUsername()
    {
        var username = Context.GetUsername();
        if (username == null)
        {
            _logger.LogInformation("Unauthenticated request by connection: {connId}", Context.ConnectionId);
            throw new AuthenticationError(Events.Errors.UnauthenticatedRequest);
        }
        return username;
    }

    private async Task AddToGroup(string playerIdentifier, string gameId)
    {
        _groupOrganizer.AddToGroup(playerIdentifier, gameId);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _logger.LogDebug("User <{user}> joined game <{gameid}> successfully", playerIdentifier, gameId);
    }

    private async Task RemoveFromGroup(string playerIdentifier, string gameId)
    {
        _groupOrganizer.RemoveFromGroup(playerIdentifier, gameId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        _logger.LogDebug("User <{user}> left game <{gameid}> successfully", playerIdentifier, gameId);
    }


    public class AuthenticationError : Exception
    {
        public string ErrorType { get; }
        public AuthenticationError(string errorType)
            : base($"Caller is not authenticated. Please sign in before making requests to the hub.")
        {
            ErrorType = errorType;
        }
    }
}


