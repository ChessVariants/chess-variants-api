using Microsoft.AspNetCore.SignalR;

namespace ChessVariantsAPI.Hubs;

public class GameHub : Hub
{
    public async Task MovePiece(string move)
    {
        // if move is valid, compute new board
        await Clients.All.SendAsync("pieceMoved", "board"); // TODO only respond to players of the particular game
        // else respond with illegal move
        //await Clients.Caller.SendAsync("illegalMove", "board");
    }
}
