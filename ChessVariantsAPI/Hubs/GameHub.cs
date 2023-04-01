using ChessVariantsAPI.GameOrganization;
using ChessVariantsAPI.Hubs.DTOs;
using ChessVariantsLogic;
using ChessVariantsLogic.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

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


    public GameHub(GameOrganizer organizer, ILogger<GameHub> logger, GroupOrganizer groupOrganizer)
    {
        _organizer = organizer;
        _groupOrganizer = groupOrganizer;
        _logger = logger;
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

    public async Task SetGame(string gameId, string variantIdentifier)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to set game-variant {variant} for game with id <{gameid}>", user, variantIdentifier, gameId);
            var result = _organizer.SetGame(gameId, variantIdentifier);
            if (result == false)
            {
                await Clients.Caller.SendGenericError($"Could not set game variant to {variantIdentifier}");
            }
            await Clients.Groups(gameId).SendGameVariantSet(variantIdentifier);
            _logger.LogDebug("User <{user}> set game variant to {variant} for game <{gameid}> result: {bool}", user, variantIdentifier, gameId, result);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to create game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
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
    public async Task CreateGame(string gameId, string variantIdentifier)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to create game with id <{gameid}>", user, gameId);
            Player createdPlayer = _organizer.CreateGame(gameId, user, variantIdentifier);
            await AddToGroup(user, gameId);
            await Clients.Caller.SendGameCreated(createdPlayer.AsString(), user);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to create game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendGenericError(e.Message);
        }
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

    /// <summary>
    /// Makes a move on the board if the move is valid and informs users of the gamestate.
    /// </summary>
    /// <param name="move">Move requested to be made</param>
    /// <param name="gameId">Id of the game</param>
    /// <returns></returns>
    public async Task MovePiece(string move, string gameId)
    {
        // if move is valid, compute new board
        GameEvent? result = null;
        GameState? state = null;
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to make move <{move}> in game <{gameid}>", user, move, gameId);
            result = _organizer.Move(move, gameId, user);
            state = _organizer.GetState(gameId);
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendGenericError(e.Message);
        }

        switch (result)
        {
            case GameEvent.InvalidMove:
                _logger.LogDebug("Move <{move}> in game <{gameid}> was invalid", move, gameId);
                await Clients.Caller.SendInvalidMove();
                return;
            case GameEvent.MoveSucceeded:
                _logger.LogDebug("Move <{move}> in game <{gameid}> was successful", move, gameId);
                await Clients.Groups(gameId).SendUpdatedGameState(state!);
                return;
            case GameEvent.WhiteWon:
                _logger.LogDebug("Move <{move}> in game <{gameid}> won the game for white", move, gameId);
                await Clients.Group(gameId).SendWhiteWon();
                return;
            case GameEvent.BlackWon:
                _logger.LogDebug("Move <{move}> in game <{gameid}> won the game for black", move, gameId);
                await Clients.Group(gameId).SendBlackWon();
                return;
            case GameEvent.Tie:
                _logger.LogDebug("Move <{move}> in game <{gameid}> resulted in a tie", move, gameId);
                await Clients.Group(gameId).SendTie();
                return;
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


    private class AuthenticationError : Exception
    {
        public string ErrorType { get; }
        public AuthenticationError(string errorType)
            : base($"Caller is not authenticated. Please sign in before making requests to the hub.")
        {
            ErrorType = errorType;
        }
    }
}


