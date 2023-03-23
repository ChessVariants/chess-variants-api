using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChessVariantsAPI.Hubs;

/// <summary>
/// A SignalR hub for handling real-time chess game communication between client and server.
/// </summary>
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
    public async Task JoinGame(string gameId)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to join game with id <{gameid}>", user, gameId);
            var joinCallerEvent = _organizer.GetGameState(gameId) == ActiveGameState.Game ? Events.GameJoined : Events.LobbyJoined;
            var joinGroupEvent = _organizer.GetGameState(gameId) == ActiveGameState.Game ? Events.PlayerJoinedGame : Events.PlayerJoinedLobby;
            if (_groupOrganizer.InGroup(user, gameId))
            {
                await AddToGroup(user, gameId);
                var player = _organizer.GetPlayer(gameId, user);
                await Clients.Groups(gameId).SendAsync(joinGroupEvent, player.AsString());
                await Clients.Caller.SendAsync(joinCallerEvent, player.AsString());
                return;
            }

            var createdPlayer = _organizer.JoinGame(gameId, user);
            await Clients.Caller.SendAsync(joinCallerEvent, createdPlayer.AsString());
            await Clients.Groups(gameId).SendAsync(joinGroupEvent, createdPlayer.AsString());
            await AddToGroup(user, gameId);
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to join game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
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
                await Clients.Caller.SendAsync(Events.Error, $"Could not set game variant to {variantIdentifier}");
            }
            await Clients.Groups(gameId).SendAsync(Events.GameVariantSet, variantIdentifier);
            _logger.LogDebug("User <{user}> set game variant to {variant} for game <{gameid}> result: {bool}", user, variantIdentifier, gameId, result);
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to create game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }
    }

    public async Task StartGame(string gameId)
    {
        try
        {
            var user = GetUsername();
            if (_organizer.GetGameState(gameId) == ActiveGameState.Lobby)
            {
                _logger.LogInformation("<{u}> tried to start game <{g}> without setting a variant", user, gameId);
                await Clients.Caller.SendAsync(Events.GameVariantNotSet, $"No variant is set for game {gameId}. Please choose a variant before starting the game.");
            }
            else
            {
                _logger.LogInformation("Started game {g}", gameId);
                await Clients.Groups(gameId).SendAsync(Events.GameStarted);
                var state = _organizer.GetStateAsJson(gameId);
                await Clients.Groups(gameId).SendAsync(Events.UpdatedGameState, state);
            }
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to start game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
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
            await Clients.Caller.SendAsync(Events.GameCreated, createdPlayer.AsString());
            _logger.LogDebug("User <{user}> joined game <{gameid}> successfully", user, gameId);
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to create game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }
    }

    public async Task CreateLobby(string gameId)
    {
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to create lobby with id <{gameid}>", user, gameId);
            var createdPlayer = _organizer.CreateLobby(gameId, user);
            await Clients.Caller.SendAsync(Events.LobbyCreated, createdPlayer.AsString());
            await AddToGroup(user, gameId);
            _logger.LogDebug("User <{user}> joined game <{gameid}> successfully", user, gameId);
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to create lobby with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
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
            await Clients.Caller.SendAsync(Events.GameLeft);
            bool playerLeft = _organizer.LeaveGame(gameId, user);
            if (playerLeft)
            {
                await Clients.Groups(gameId).SendAsync(Events.PlayerLeftGame, Context.GetUsername());
            }
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (GameEmptyException)
        {
            _organizer.DeleteGame(gameId);
            _logger.LogDebug("Game with id <{gameid}> was deleted", gameId);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to leave game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e);
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
            await Clients.Group(gameId).SendAsync(Events.ColorsSwapped);
            _logger.LogDebug("Colors swapped in game <{gameid}>", gameId);
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When trying to swap colors in game with id <{gameid}> the following error occured: {error}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
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
        string? state = null;
        try
        {
            var user = GetUsername();
            _logger.LogDebug("User <{user}> trying to make move <{move}> in game <{gameid}>", user, move, gameId);
            result = _organizer.Move(move, gameId, user);
            state = _organizer.GetStateAsJson(gameId);
        }
        catch (AuthenticationError e)
        {
            await Clients.Caller.SendAsync(e.ErrorType, e.Message);
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }

        switch (result)
        {
            case GameEvent.InvalidMove:
                _logger.LogDebug("Move <{move}> in game <{gameid}> was invalid", move, gameId);
                await Clients.Caller.SendAsync(Events.InvalidMove);
                return;
            case GameEvent.MoveSucceeded:
                _logger.LogDebug("Move <{move}> in game <{gameid}> was successful", move, gameId);
                await Clients.Groups(gameId).SendAsync(Events.UpdatedGameState, state);
                await Clients.Caller.SendAsync(Events.UpdatedGameState, state);
                return;
            case GameEvent.WhiteWon:
                _logger.LogDebug("Move <{move}> in game <{gameid}> won the game for white", move, gameId);
                await Clients.Group(gameId).SendAsync(Events.WhiteWon);
                return;
            case GameEvent.BlackWon:
                _logger.LogDebug("Move <{move}> in game <{gameid}> won the game for black", move, gameId);
                await Clients.Group(gameId).SendAsync(Events.BlackWon);
                return;
            case GameEvent.Tie:
                _logger.LogDebug("Move <{move}> in game <{gameid}> resulted in a tie", move, gameId);
                await Clients.Group(gameId).SendAsync(Events.Tie);
                return;
        }
    }

    /// <summary>
    /// Responds to the caller with the current game state
    /// </summary>
    /// <param name="gameId">The game to request state for</param>
    /// <returns></returns>
    public async Task RequestState(string gameId)
    {
        try
        {
            _logger.LogDebug("Board state for game <{gameid}> was requested", gameId);
            var state = _organizer.GetStateAsJson(gameId);
            await Clients.Caller.SendAsync(Events.UpdatedGameState, state);
        }
        catch (OrganizerException e)
        {
            _logger.LogInformation("When requesting board state for game <{gameid}> the following error occured: {e}", gameId, e.Message);
            await Clients.Caller.SendAsync(Events.Error, e.Message);
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


    private static class Events
    {
        public readonly static string GameNotFound = "gameNotFound";
        public readonly static string PieceMoved = "pieceMoved";
        public readonly static string UpdatedGameState = "updatedGameState";
        public readonly static string PlayerLeftGame = "playerLeftGame";
        public readonly static string PlayerJoinedGame = "playerJoinedGame";
        public readonly static string GameCreated = "gameCreated";
        public readonly static string GameJoined = "gameJoined";
        public readonly static string GameLeft = "gameLeft";
        public readonly static string PlayerNotFound = "playerNotFound";
        public readonly static string InvalidMove = "invalidMove";
        public readonly static string Error = "error";
        public readonly static string WhiteWon = "whiteWon";
        public readonly static string BlackWon = "blackWon";
        public readonly static string Tie = "tie";
        public readonly static string ColorsSwapped = "colorsSwapped";

        // Lobby events
        public readonly static string PlayerJoinedLobby = "playerJoinedLobby";
        public readonly static string PlayerLeftLobby = "playerLeftLobby";
        public readonly static string LobbyCreated = "lobbyCreated";
        public readonly static string LobbyJoined = "lobbyJoined";
        public readonly static string GameVariantSet = "gameVariantSet";
        public readonly static string GameVariantNotSet = "gameVariantNotSet";
        public readonly static string GameStarted = "gameStarted";

        public static class Errors
        {
            public readonly static string UnauthenticatedRequest = "unauthenticatedRequest";
        }
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


