using ChessVariantsLogic.Export;

namespace ChessVariantsLogic.Editor;

public class PieceEditor
{

    private readonly PieceBuilder _builder;
    private Piece? _piece;

    public PieceEditor()
    {
        this._builder = new PieceBuilder();
    }

    public string GetAllCurrentlyValidMovesFromSquareAsJson(string square)
    {
        var moves = _builder.GetAllCurrentlyValidMovesFromSquare(square);
        return PieceExporter.ExportLegalMovesAsJson(moves);
    }

    public string GetAllCurrentlyValidCapturesFromSquareAsJson(string square)
    {
        var moves = _builder.GetAllCurrentlyValidCaptureMovesFromSquare(square);
        return PieceExporter.ExportLegalMovesAsJson(moves);
    }

    public void AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
            _builder.AddJumpMovementPattern(xDir, yDir);
        else
            _builder.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
            _builder.AddJumpCapturePattern(xDir, yDir);
        else
            _builder.AddCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public void RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
            _builder.RemoveJumpMovementPattern(xDir, yDir);
        else
            _builder.RemoveMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public void RemoveCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
            _builder.RemoveJumpCapturePattern(xDir, yDir);
        else
            _builder.RemoveCapturePattern(xDir, yDir, minLength, maxLength);
    }

    public EditorEvent BelongsToPlayer(string player)
    {
        try
        {
            _builder.BelongsToPlayer(player);
        }
        catch(ArgumentException)
        {
            return EditorEvent.UnknownPlayer;
        }
        return EditorEvent.Success;
    }

    public void SetSameMovementAndCapturePattern(bool enable)
    { 
        _builder.SetSameMovementAndCapturePattern(enable);
    }
    public void SetCanBeCaptured(bool enable) { _builder.SetCanBeCaptured(enable); }

    public void RepeatMovement(int repeat) { _builder.RepeatMovement(repeat); }

    public void SetRoyal(bool enable) { _builder.SetRoyal(enable); }

    public EditorEvent BuildPiece()
    {
        try 
        {
            _piece = _builder.Build();
        }
        catch (ArgumentException)
        {
            return EditorEvent.BuildFailed;
        }
        return EditorEvent.Success;
    }

    public string ExportPieceAsJson()
    {
        if(_piece != null)
            return _piece.ExportAsJson();
        throw new ArgumentNullException("Piece has not been built successfully.");
    }

    public void ResetPiece()
    {
        _builder.Reset();
    }

}

public enum EditorEvent
{
    Success,
    InvalidMovementPattern,
    BuildFailed,
    UnknownPlayer,
}