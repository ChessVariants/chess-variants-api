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

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _organizer.CreateNewGame(gameId);
        await Clients.Groups(gameId).SendAsync("playerJoinedGame", Context.ConnectionId);
    }

    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.Groups(gameId).SendAsync("playerLeftGame", Context.ConnectionId);
    }

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

    public async Task RequestBoardState()
    {
        await Clients.Caller.SendAsync("updatedBoardState", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
    }
}
