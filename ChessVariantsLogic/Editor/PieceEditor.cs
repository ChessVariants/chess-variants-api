namespace ChessVariantsLogic.Editor;

public class PieceEditor
{

    private readonly PieceBuilder _builder;
    private Piece? _piece;

    public PieceEditor()
    {
        this._builder = new PieceBuilder();
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

    public void ResetPiece()
    {
        _builder.Reset();
    }

    public string ExportStateAsJson()
    {
        return "Hey, it works!";
        
        //RuleSet rules = _playerTurn == Player.White ? _whiteRules : _blackRules;
        //return GameExporter.ExportGameStateAsJson(_moveWorker.Board, _playerTurn, rules.GetLegalMoveDict(_playerTurn, _moveWorker));
    }

}

public enum EditorEvent
{
    Success,
    InvalidMovementPattern,
    BuildFailed,
    UnknownPlayer,
}