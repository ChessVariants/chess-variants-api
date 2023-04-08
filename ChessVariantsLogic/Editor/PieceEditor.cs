using ChessVariantsLogic.Export;

namespace ChessVariantsLogic.Editor;

/// <summary>
/// This class represents an editor for creating a new object of type <see cref="Piece"/>.
/// </summary>
public class PieceEditor
{

    private readonly PieceBuilder _builder;
    private Piece? _piece;
    private MoveWorker _moveWorker;
    
    private Piece _dummy;
    private string _square;

    private bool _showMovement;

    public PieceEditor()
    {
        this._builder = new PieceBuilder();
        this._moveWorker = new MoveWorker(new Chessboard(8));
        this._square = "e4";
        this._dummy = _builder.GetDummyPieceWithCurrentMovement();
        this._showMovement = true;

        _moveWorker.InsertOnBoard(_dummy, _square);
    }

    public void ShowMovement() { _showMovement = true; }
    public void ShowCaptures() { _showMovement = false; }

    public EditorState GetCurrentState()
    {
        if(_showMovement)
            return EditorExporter.ExportEditorState(_moveWorker.Board, Player.White, GetAllCurrentlyValidMoves(), _square);
        else
            return EditorExporter.ExportEditorState(_moveWorker.Board, Player.White, GetAllCurrentlyValidCaptures(), _square);
    }

    public void UpdateBoardSize(int row, int col)
    {
        _moveWorker.Board = new Chessboard(row, col);
    }

    public void SetActiveSquare(string square)
    {
        _moveWorker.RemoveFromBoard(_square);
        this._square = square;
        _moveWorker.InsertOnBoard(_dummy, _square);
    }

    /// <summary>
    /// Genereates all valid moves from the current state of the builder.
    /// </summary>
    /// <param name="square">is the square where the piece should be inserted on.</param>
    /// <returns>A HashSet of all the currently valid moves from the square <paramref name="square"/>.</returns>
    public HashSet<string> GetAllCurrentlyValidMoves()
    {
        _dummy = _builder.GetDummyPieceWithCurrentMovement();
        if(_moveWorker.InsertOnBoard(_dummy, _square))
            return _moveWorker.GetAllValidMoves(Player.White);
        throw new ArgumentException("Invalid square.");
    }

    public HashSet<string> GetAllCurrentlyValidCaptures()
    {
        if(_builder.HasSameMovementAndCapturePattern())
            return GetAllCurrentlyValidMoves();
        _dummy = _builder.GetDummyPieceWithCurrentCaptures(); // Does not work due to pieces being saved by their classifiers when looking up their movement pattern.
        if(_moveWorker.InsertOnBoard(_dummy, _square))
        {
            var test = _moveWorker.GetAllValidMoves(Player.White);
            return test;
        }
        throw new ArgumentException("Invalid square.");
    }

    /// <summary>
    /// Returns a string in json-format of all valid moves from the current state of the builder.
    /// </summary>
    /// <param name="square">is the square on which the moves are calculated.</param>
    /// <returns>A json-string of all valid moves.</returns>
    //public string GetAllCurrentlyValidMovesFromSquareAsJson(string square)
    //{
    //    var moves = GetAllCurrentlyValidMoves();
    //    return PieceExporter.ExportLegalMovesAsJson(moves);
    //}

    /// <summary>
    /// Returns a string in json-format of all valid capture-moves from the current state of the builder.
    /// </summary>
    /// <param name="square">is the square on which the capture-moves are calculated.</param>
    /// <returns>A json-string of all valid capture-moves.</returns>
    //public string GetAllCurrentlyValidCapturesFromSquareAsJson(string square)
    //{
    //    var moves = _builder.GetAllCurrentlyValidCaptureMovesFromSquare(square);
    //    return PieceExporter.ExportLegalMovesAsJson(moves);
    //}

    /// <summary>
    /// Adds a pattern to the set of allowed movement. 
    /// </summary>
    /// <param name="xDir">is the direction on the x-axis.</param>
    /// <param name="yDir">is the direction on the y-axis.</param>
    /// <param name="minLength">is the minimum length.</param>
    /// <param name="maxLength">is the maximum length.</param>
    public void AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
            _builder.AddJumpMovementPattern(xDir, yDir);
        else
            _builder.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    /// <summary>
    /// Adds a pattern to the set of allowed captures. 
    /// </summary>
    /// <param name="xDir">is the direction on the x-axis.</param>
    /// <param name="yDir">is the direction on the y-axis.</param>
    /// <param name="minLength">is the minimum length.</param>
    /// <param name="maxLength">is the maximum length.</param>
    public void AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
            _builder.AddJumpCapturePattern(xDir, yDir);
        else
            _builder.AddCapturePattern(xDir, yDir, minLength, maxLength);
    }

    /// <summary>
    /// Removes a pattern to the set of currently allowed movement. 
    /// </summary>
    /// <param name="xDir">is the direction on the x-axis.</param>
    /// <param name="yDir">is the direction on the y-axis.</param>
    /// <param name="minLength">is the minimum length.</param>
    /// <param name="maxLength">is the maximum length.</param>
    public EditorEvent RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
        {
            if(_builder.RemoveJumpMovementPattern(xDir, yDir))
                return EditorEvent.Success;
        }
        else
        {
            if(_builder.RemoveMovementPattern(xDir, yDir, minLength, maxLength))
                return EditorEvent.Success;
        }
        return EditorEvent.NoPatternRemoved;
    }

    /// <summary>
    /// Removes a pattern to the set of currently allowed captures. 
    /// </summary>
    /// <param name="xDir">is the direction on the x-axis.</param>
    /// <param name="yDir">is the direction on the y-axis.</param>
    /// <param name="minLength">is the minimum length.</param>
    /// <param name="maxLength">is the maximum length.</param>
    public EditorEvent RemoveCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        if(minLength < 0)
        {
            if(_builder.RemoveJumpCapturePattern(xDir, yDir))
                return EditorEvent.Success;
        }
        else
        {
            if(_builder.RemoveCapturePattern(xDir, yDir, minLength, maxLength))
                return EditorEvent.Success;
        }
        return EditorEvent.NoPatternRemoved;
    }

    /// <summary>
    /// Sets the player that the piece should belong to, i.e. "white", "black", or "shared".
    /// </summary>
    /// <param name="player">is a string representation of the player.</param>
    /// <returns>An EditorEvent describing if the method was successful or not.</returns>
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

    /// <summary>
    /// Set true if the Piece should have the same capture and movement patterns, false to have separate patterns. Is true from the preset.
    /// </summary>
    /// <param name="enable">true to enable same capture and movement pattern, false to disable.</param>
    public void SetSameMovementAndCapturePattern(bool enable) { _builder.SetSameMovementAndCapturePattern(enable); }

    /// <summary>
    /// Set true if the piece can be captured, false if it can not. Is true from the preset.
    /// </summary>
    /// <param name="enable">true to allow piece to be captured, otherwise false.</param>
    public void SetCanBeCaptured(bool enable) { _builder.SetCanBeCaptured(enable); }

    /// <summary>
    /// Set how many times the movement pattern can be repeated. Argument must be between 0 and 3.
    /// </summary>
    /// <param name="repeat">is the amount of times the movement pattern should be repeated.</param>
    public void RepeatMovement(int repeat) { _builder.RepeatMovement(repeat); }

    /// <summary>
    /// Set true if the piece should be royal.
    /// </summary>
    /// <param name="enable">true if the piece is royal, otherwise false.</param>
    public void SetRoyal(bool enable) { _builder.SetRoyal(enable); }

    /// <summary>
    /// Builds the piece with the current settings.
    /// </summary>
    /// <returns>EditorEvent.Success if the build was successful, otherwise EditorEvent.BuildFailed.</returns>
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

    /// <summary>
    /// Returns the piece as a string of Json-format.
    /// </summary>
    /// <returns>If the piece is successfully built it returns astring on Json-format, otherwise ArgumentNullException is surfaced.</returns>
    public string ExportStateAsJson()
    {
        if(_piece != null)
            return _piece.ExportAsJson();
        throw new ArgumentNullException("Piece has not been built successfully.");
    }

    /// <summary>
    /// Resets the current state of the builder to its original state.
    /// </summary>
    public void ResetPiece()
    {
        _builder.Reset();
    }

}

/// <summary>
/// Describes an event that occured in an editor.
/// </summary>
public enum EditorEvent
{
    Success,
    BuildFailed,
    UnknownPlayer,
    NoPatternRemoved,
}