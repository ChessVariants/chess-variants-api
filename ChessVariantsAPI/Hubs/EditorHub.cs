using Microsoft.AspNetCore.SignalR;
using ChessVariantsLogic.Export;
using DataAccess.MongoDB;
using ChessVariantsAPI.ObjectTranslations;
using Microsoft.AspNetCore.Authorization;
using ChessVariantsAPI.Hubs.DTOs;
using DataAccess.MongoDB.Models;

namespace ChessVariantsAPI.Hubs;

[Authorize]
public class EditorHub : Hub
{

    private readonly EditorOrganizer _organizer;
    readonly protected ILogger _logger;
    private DatabaseService _db;

    //Stanard pieces are for now stored in the db under this user
    private static readonly string StandardPieceUser = "admin";

    public EditorHub(EditorOrganizer organizer, ILogger<EditorHub> logger, DatabaseService databaseService)
    {
        _organizer = organizer;
        _logger = logger;
        _db = databaseService;
    }

    #region BoardEditor

    public override Task OnConnectedAsync()
    {
        _logger.LogDebug("Connected editor");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("disconnected editor");
        return base.OnDisconnectedAsync(exception);
    }

    public BoardEditorState RequestBoardEditorState(string editorId) { return _organizer.GetCurrentBoardEditorState(editorId); }
    public async Task CreateBoardEditor(string editorId)
    {
        _organizer.CreateBoardEditor(editorId);
        await UpdateBoardEditorState(editorId);
    }

    private async Task UpdateBoardEditorState(string editorId)
    {
        var state = _organizer.GetCurrentBoardEditorState(editorId);
        await Clients.Caller.SendUpdatedBoardEditorState(state);
    }

    public async Task SetBoardEditorSize(string editorId, int row, int col)
    {
        _organizer.SetBoardEditorBoardSize(editorId, row, col);
        await UpdateBoardEditorState(editorId);
    }

    public async Task SetActivePiece(string editorId, string pieceName)
    {
        var user = GetUsername();
        Piece? pieceModel = null;
        try
        {
            pieceModel = await _db.Pieces.GetByUserAndPieceNameAsync(StandardPieceUser, pieceName);
        }
        catch (InvalidOperationException)
        {
            _logger.LogDebug("Piece {pName} is not a standard piece", pieceName);
        }
        try
        {
            pieceModel = await _db.Pieces.GetByUserAndPieceNameAsync(user, pieceName);
        }
        catch (InvalidOperationException)
        {
            _logger.LogDebug("Piece {pName} is not a piece created bu {user}", pieceName, user);
        }
        if (pieceModel == null)
        {
            await Clients.Caller.SendCouldNotFetchPiece();
            return;
        }
        var logicPiece = PieceTranslator.CreatePieceLogic(pieceModel);
        _organizer.SetActivePiece(editorId, logicPiece);
        await UpdateBoardEditorState(editorId);
    }

    public async Task SetActiveRemove(string editorId)
    {
        _organizer.SetActiveRemove(editorId);
        await UpdateBoardEditorState(editorId);
    }

    public async Task UpdateSquare(string editorId, string square)
    {
        _organizer.InsertPiece(editorId, square);
        await UpdateBoardEditorState(editorId);
    }

    public async Task ResetStartingPosition(string editorId)
    {
        _organizer.ResetStartingPosition(editorId);
        await UpdateBoardEditorState(editorId);
    }

    public async Task ClearBoard(string editorId)
    {
        _organizer.ClearBoard(editorId);
        await UpdateBoardEditorState(editorId);
    }

    public async Task BuildChessboard(string editorId, string boardName)
    {
        var user = GetUsername();
        try
        {
            var userBoard = await _db.Chessboards.GetByUserAndBoardNameAsync(user, boardName);
            await Clients.Caller.SendBuildFailed("User <"+ userBoard.Creator + "> already has created a chessboard with the name <" + userBoard.Name + ">. Try a different name.");
            return;
        }
        catch (InvalidOperationException) {}

        var board = _organizer.BuildBoard(editorId);
        var boardModel = ChessboardTranslator.CreateChessboardModel(board, boardName, user);

        _logger.LogDebug("User <{user}> attempts to save board <{boardName}> to database.", user, boardName);
        await _db.Chessboards.CreateAsync(boardModel);
        _logger.LogDebug("User <{user}> saved board <{boardName}> to database.", user, boardName);
    }

    public async Task<List<string>> RequestUserChessboards()
    {
        var user = GetUsername();
        var modelBoards = await _db.Chessboards.GetByUserAsync(user);
        var boards = new List<string>();
        foreach (var b in modelBoards)
        {
            boards.Add(b.Name);
        }
        return boards;
    }


    #endregion


    #region PieceEditor

    public async Task CreatePieceEditor(string editorId)
    {
        _organizer.CreatePieceEditor(editorId);
        await UpdatePieceEditorState(editorId);
        await UpdatePatternState(editorId);
    }

    public async Task SetImagePath(string editorId, string imagePath)
    {
        _organizer.SetImagePath(editorId, imagePath);
        await UpdatePieceEditorState(editorId);
    }

    public PieceEditorState RequestPieceEditorState(string editorId) { return _organizer.GetCurrentPieceEditorState(editorId); }
    public PatternState RequestPatternState(string editorId) { return _organizer.GetCurrentPatternState(editorId); }

    private async Task UpdatePatternState(string editorId)
    {
        var patternState = _organizer.GetCurrentPatternState(editorId);
        await Clients.Caller.SendUpdatedPatternState(patternState);
    }

    private async Task UpdatePieceEditorState(string editorId)
    {
        var state = _organizer.GetCurrentPieceEditorState(editorId);
        await Clients.Caller.SendUpdatedPieceEditorState(state);
    }

    public async Task ActivateSquare(string editorId, string square)
    {
        _organizer.SetActiveSquare(editorId, square);
        await UpdatePieceEditorState(editorId);
    }

    public async Task AddMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddMovementPattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task RemoveMovementPattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.RemoveMovementPattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task AddCapturePattern(string editorId, int xDir, int yDir, int minLength, int maxLength)
    {
        _organizer.AddCapturePattern(editorId, xDir, yDir, minLength, maxLength);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task UpdatePieceEditorBoardSize(string editorId, int rows, int cols)
    {
        _organizer.SetPieceEditorBoardSize(editorId, rows, cols);
        await UpdatePieceEditorState(editorId);
    }

    public async Task SetCaptureSameAsMovement(string editorId, bool enable)
    {
        _organizer.SameMovementAndCapture(editorId, enable);
        await UpdatePieceEditorState(editorId);
    }

    public async Task SetCanBePromotedTo(string editorId, bool enable)
    {
        _organizer.CanBePromotedTo(editorId, enable);
        await UpdatePieceEditorState(editorId);
    }

    public async Task ShowMovement(string editorId, bool enable)
    {
        _organizer.ShowMovement(editorId, enable);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task PieceCanBeCaptured(string editorId, bool enable)
    {
        _organizer.CanBeCaptured(editorId, enable);
        await UpdatePieceEditorState(editorId);
    }

    public async Task BelongsToPlayer(string editorId, string player)
    {
        _organizer.BelongsToPlayer(editorId, player);
        await UpdatePieceEditorState(editorId);
    }

    public async Task AmountRepeat(string editorId, int repeat)
    {
        _organizer.RepeatMovement(editorId, repeat);
        await UpdatePieceEditorState(editorId);
    }

    public async Task ClearMovementPatterns(string editorId)
    {
        _organizer.RemoveAllMovementPatterns(editorId);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task ResetPiece(string editorId)
    {
        _organizer.Reset(editorId);
        await UpdatePatternState(editorId);
        await UpdatePieceEditorState(editorId);
    }

    public async Task BuildPiece(string editorId, string pieceName)
    {
        var user = GetUsername();

        try
        {
            var userPiece = await _db.Pieces.GetByUserAndPieceNameAsync(user, pieceName);
            await Clients.Caller.SendBuildFailed("User <"+ userPiece.Creator + "> already has created a piece with the name <" + userPiece.Name + ">. Try a different name.");
            return;
        }
        catch (InvalidOperationException) {}

        ChessVariantsLogic.Piece? piece;
        try
        {
            piece = _organizer.Build(editorId);
        }
        catch (ArgumentException e)
        {
            await Clients.Caller.SendBuildFailed(e.Message);
            return;
        }
        var modelPiece = PieceTranslator.CreatePieceModel(piece, pieceName, user, piece.ImagePath);
        _logger.LogDebug("Attempting to save piece <{pieceName}> by user <{user}>", pieceName, user);
        await _db.Pieces.CreateAsync(modelPiece);
        _logger.LogDebug("Piece <{pieceName}> saved to database by user <{user}>.", pieceName, user);

    }

    public async Task<List<PieceDTO>> RequestUserPieces()
    {
        var user = GetUsername();
        var pieces = await _db.Pieces.GetByUserAsync(user);
        var pieceDTOs = new List<PieceDTO>();
        foreach (var p in pieces)
        {
            pieceDTOs.Add(new PieceDTO{ Name = p.Name, Image = p.ImagePath});
        }
        return pieceDTOs;
    }

    public async Task<List<PieceDTO>> RequestStandardPiecesByColor(string color)
    {
        var pieces = await _db.Pieces.GetStandardPieces();
        var pieceDTOs = new List<PieceDTO>();
        foreach (var p in pieces)
        {
            if (!p.BelongsTo.Equals(color))
                continue;
            pieceDTOs.Add(new PieceDTO { Name = p.Name, Image = p.ImagePath});
        }

        return pieceDTOs;
    }

    private string GetUsername()
    {
        var username = Context.GetUsername();
        if (username == null)
        {
            _logger.LogInformation("Unauthenticated request by connection: {connId}", Context.ConnectionId);
            throw new GameHub.AuthenticationError(Events.Errors.UnauthenticatedRequest);
        }
        return username;
    }

    #endregion

}