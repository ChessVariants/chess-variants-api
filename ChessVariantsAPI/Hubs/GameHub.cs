using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ChessVariantsAPI.Hubs;

/// <summary>
/// A SignalR hub for handling real-time chess game communication between client and server.
/// </summary>
public class GameHub : Hub
{
    private readonly GameOrganizer _organizer;
    readonly protected ILogger _logger;


    public GameHub(GameOrganizer organizer, ILogger<GameHub> logger)
    {
        _organizer = organizer;
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
            Player createdPlayer = _organizer.JoinGame(gameId, user);
            await Clients.Caller.SendAsync(Events.GameJoined, createdPlayer.AsString());
            await Groups.AddToGroupAsync(user, gameId);
            await Clients.Groups(gameId).SendAsync(Events.PlayerJoinedGame, user);
            _logger.LogDebug("User <{user}> joined game <{gameid}> successfully", user, gameId);
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
            await Clients.Caller.SendAsync(Events.GameCreated, createdPlayer.AsString());
            await Groups.AddToGroupAsync(user, gameId);
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
            await Groups.RemoveFromGroupAsync(user, gameId);
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


