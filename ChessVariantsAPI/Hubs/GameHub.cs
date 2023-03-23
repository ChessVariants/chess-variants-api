using ChessVariantsAPI.GameOrganization;
using ChessVariantsLogic;
using Microsoft.AspNetCore.SignalR;

namespace ChessVariantsAPI.Hubs;

/// <summary>
/// A SignalR hub for handling real-time chess game communication between client and server.
/// </summary>
public class GameHub : Hub
{
    private readonly GameOrganizer _organizer;

    public GameHub(GameOrganizer organizer)
    {
        _organizer = organizer;
    }

    /// <summary>
    /// Adds the caller to a group corresponding to the supplied <paramref name="gameId"/>.
    /// </summary>
    /// <param name="gameId">The id for the game to join</param>
    /// <returns></returns>
    public async Task JoinGame(string gameId)
    {
        try
        {
            Player createdPlayer = _organizer.JoinGame(gameId, Context.ConnectionId);
            await Clients.Caller.SendAsync(Events.GameJoined, createdPlayer.AsString());
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Groups(gameId).SendAsync(Events.PlayerJoinedGame, Context.ConnectionId);
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }
    }

    /// <summary>
    /// Adds the caller to a group corresponding to the supplied <paramref name="gameId"/> and creates the game.
    /// </summary>
    /// <param name="gameId">The id for the game to join</param>
    /// <param name="variantIdentifier">What variant to create</param>
    /// <returns></returns>
    public async Task CreateGame(string gameId, string variantIdentifier)
    {
        try
        {
            Player createdPlayer = _organizer.CreateGame(gameId, Context.ConnectionId, variantIdentifier);
            await Clients.Caller.SendAsync(Events.GameCreated, createdPlayer.AsString());
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }
        catch (OrganizerException e)
        {
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
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            await Clients.Caller.SendAsync(Events.GameLeft);
            bool playerLeft = _organizer.LeaveGame(gameId, Context.ConnectionId);
            if (playerLeft)
            {
                await Clients.Groups(gameId).SendAsync(Events.PlayerLeftGame, Context.ConnectionId);
            }
        }
        catch (GameEmptyException)
        {
            _organizer.DeleteGame(gameId);
        }
        catch (OrganizerException e)
        {
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
            _organizer.SwapColors(gameId, Context.ConnectionId);
        }
        catch (OrganizerException e)
        {
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
        ISet<GameEvent>? result = null;
        string? state = null;
        try
        {
            result = _organizer.Move(move, gameId, Context.ConnectionId);
            state = _organizer.GetStateAsJson(gameId);
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }

        if(result.Contains(GameEvent.InvalidMove))
        {
            await Clients.Caller.SendAsync(Events.InvalidMove);
            return;
        }
        if(result.Contains(GameEvent.MoveSucceeded))
        {
            await Clients.Groups(gameId).SendAsync(Events.UpdatedGameState, state);
        }

        if (result.Contains(GameEvent.WhiteWon))
        {
            await Clients.Group(gameId).SendAsync(Events.WhiteWon);
        }
        else if(result.Contains(GameEvent.BlackWon))
        {
            await Clients.Group(gameId).SendAsync(Events.BlackWon);
        }
        else if(result.Contains(GameEvent.Tie))
        {
            await Clients.Group(gameId).SendAsync(Events.Tie);
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
            var state = _organizer.GetStateAsJson(gameId);
            await Clients.Caller.SendAsync(Events.UpdatedGameState, state);
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }
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
    }
}


