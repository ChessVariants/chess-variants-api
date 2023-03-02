using ChessVariantsAPI.GameOrganization;
using Microsoft.AspNetCore.SignalR;
using ChessVariantsLogic;

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
    public async Task JoinGame(string gameId, string asColor)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        _organizer.JoinGame(gameId, Context.ConnectionId, asColor);
        await Clients.Groups(gameId).SendAsync("playerJoinedGame", Context.ConnectionId);
    }

    /// <summary>
    /// Removes the caller from a group corresponding to the supplied <paramref name="gameId"/>. Invokes a playerLeftGame event to all clients in the joined group.
    /// </summary>
    /// <param name="gameId">The id for the game to leave</param>
    /// <returns></returns>
    public async Task LeaveGame(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        _organizer.LeaveGame(gameId, Context.ConnectionId);
        await Clients.Groups(gameId).SendAsync("playerLeftGame", Context.ConnectionId);
    }

    /// <summary>
    /// Makes a move on the board if the move is valid and informs users of the gamestate.
    /// </summary>
    /// <param name="move">Move requested to be made</param>
    /// <param name="gameId">Id of the game</param>
    /// <returns></returns>
    public async Task MovePiece(string move, string gameId)
    {
        try
        {
            var game = _organizer.GetGame(gameId);
            var player = _organizer.GetPlayer(gameId, Context.ConnectionId);
            GameEvent gameEvent = game.MakeMove(move, player);
            // Take care of GameEvent and new board state
            switch (gameEvent)
            {
                case GameEvent.WhiteWon:
                    await Clients.Groups(gameId).SendAsync("White won the game", "board");
                    break;
                case GameEvent.BlackWon:
                    await Clients.Groups(gameId).SendAsync("Black won the game", "board");
                    break;
                case GameEvent.Tie:
                    await Clients.Groups(gameId).SendAsync("The game ended in a tie", "board");
                    break;
                case GameEvent.MoveSucceeded:
                    await Clients.Groups(gameId).SendAsync("pieceMoved", "board");
                    break;
                case GameEvent.InvalidMove:
                    await Clients.Groups(gameId).SendAsync("invalidMove", "board");
                    break;
            }
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
