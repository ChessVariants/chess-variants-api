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
    /// Adds the caller to a group corresponding to the supplied <paramref name="gameId"/>. Invokes a playerJoinedGame event to all clients in the joined group.
    /// </summary>
    /// <param name="gameId">The id for the game to join</param>
    /// <returns></returns>
    public async Task JoinGame(string gameId)
    {
        try
        {
            Player createdPlayer = _organizer.JoinGame(gameId, Context.ConnectionId);
            await Clients.Caller.SendAsync(Events.GameJoined, PlayerToString(createdPlayer));
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.Groups(gameId).SendAsync(Events.PlayerJoinedGame, Context.ConnectionId);
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }
    }

    /// <summary>
    /// Adds the caller to a group corresponding to the supplied <paramref name="gameId"/> and creates the game if possible.
    /// </summary>
    /// <param name="gameId">The id for the game to join</param>
    /// <returns></returns>
    public async Task CreateGame(string gameId)
    {
        try
        {
            Player createdPlayer = _organizer.CreateGame(gameId, Context.ConnectionId);
            await Clients.Caller.SendAsync(Events.GameCreated, PlayerToString(createdPlayer));
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
    /// Placeholder method for moving a piece
    /// </summary>
    /// <param name="move"></param>
    /// <param name="gameId"></param>
    /// <returns></returns>
    public async Task MovePiece(string move, string gameId)
    {
        // if move is valid, compute new board
        GameEvent? result = null;
        try
        {
            result = _organizer.Move(move, gameId, Context.ConnectionId);
        }
        catch (OrganizerException e)
        {
            await Clients.Caller.SendAsync(Events.Error, e.Message);
        }

        switch (result)
        {
            case GameEvent.InvalidMove:
                await Clients.Caller.SendAsync(Events.InvalidMove);
                return;
            case GameEvent.MoveSucceeded:
                await Clients.Groups(gameId).SendAsync(Events.UpdatedGameState, "INSERT JSON FORMATTED STRING HERE");
                return;
            case GameEvent.WhiteWon:
                await Clients.Group(gameId).SendAsync(Events.WhiteWon);
                return;
            case GameEvent.BlackWon:
                await Clients.Group(gameId).SendAsync(Events.BlackWon);
                return;
            case GameEvent.Tie:
                await Clients.Group(gameId).SendAsync(Events.Tie);
                return;
        }
    }

    private static string PlayerToString(Player p)
    {
        switch (p)
        {
            case Player.White:
                return "white";
            case Player.Black:
                return "black";
            default:
                throw new ArgumentException("Player must be either white or black");
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


