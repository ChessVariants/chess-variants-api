using ChessVariantsAPI.GameOrganization;
using Microsoft.AspNetCore.SignalR;

namespace ChessVariantsAPI.Hubs;

public class GameHub : Hub
{
    private readonly GameOrganizer _organizer;

    public GameHub(GameOrganizer organizer)
    {
        _organizer = organizer;
    }

    /// <summary>
    /// Adds the caller to a group corresponding to the supplied game id. Invokes a playerJoinedGame event to all clients in the joined group.
    /// </summary>
    /// <param name="gameId">The id for the game to join</param>
    /// <returns></returns>
    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _organizer.CreateNewGame(gameId);
        await Clients.Groups(gameId).SendAsync("playerJoinedGame", Context.ConnectionId);
    }

    /// <summary>
    /// Removes the caller from a group corresponding to the supplied game id. Invokes a playerLeftGame event to all clients in the joined group.
    /// </summary>
    /// <param name="gameId">The id for the game to leave</param>
    /// <returns></returns>
    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.Groups(gameId).SendAsync("playerLeftGame", Context.ConnectionId);
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
        try
        {
            var game = _organizer.GetGame(gameId);
            await Clients.Groups(gameId).SendAsync("pieceMoved", "board");
        }
        catch (GameNotFoundException)
        {
            await Clients.Caller.SendAsync("gameNotFound");
        }
    }

    /// <summary>
    /// Sends a updatedBoardState event to the caller with the current board state.
    /// </summary>
    /// <returns></returns>
    public async Task RequestBoardState()
    {
        await Clients.Caller.SendAsync("updatedBoardState", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
    }
}
