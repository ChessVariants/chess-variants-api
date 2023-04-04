using ChessVariantsLogic.Export;
using Newtonsoft.Json;

namespace ChessVariantsLogic;

/// <summary>
/// A builder for the class Piece that facilitates the process of creating new and intricate pieces.
/// </summary>
public class PieceBuilder
{
    private MovementPattern movementPattern;
    private MovementPattern capturePattern;
    private bool royal;
    private PieceClassifier pc;
    private int repeat;
    private bool canBeCaptured;

    private bool sameCaptureAsMovement;

    private static string whiteCustomPieceIdentifier = "CA"; // Reset these values when a game is initialized to keep each identifier unique.
    private static string blackCustomPieceIdentifier = "ca";

    public PieceBuilder()
    {
        this.movementPattern = new MovementPattern();
        this.capturePattern = new MovementPattern();
        this.royal = false;
        this.pc = PieceClassifier.WHITE;
        this.repeat = 0;
        this.canBeCaptured = true;
        this.sameCaptureAsMovement = true;
    }

    /// <summary>
    /// Parses <paramref name="json"/> into an object of type Piece.
    /// </summary>
    /// <param name="json">is the string that should be parsed.</param>
    /// <returns>If <paramref name="json"/> is valid json format a Piece is returned, otherwise a JsonException is surfaced. </returns>
    /// <exception cref="JsonException">description</exception>
    public static Piece ParseJson(string json) //Should this be in PieceExporter?
    {
        var pieceState = JsonConvert.DeserializeObject<PieceState>(json,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

        if(pieceState == null)
            throw new JsonException();
        return Piece.ParseState(pieceState);
    }

    /// <summary>
    /// Resets the current state of the builder to its original state.
    /// </summary>
    public void Reset()
    {
        this.movementPattern = new MovementPattern();
        this.capturePattern = new MovementPattern();
        this.royal = false;
        this.pc = PieceClassifier.WHITE;
        this.repeat = 0;
        this.canBeCaptured = true;
        this.sameCaptureAsMovement = true;
    }

    /// <summary>
    /// Builds a Piece from the current state.
    /// </summary>
    /// <returns>A object of type <paramref name="Piece"/> if all fields are instantiated correctly, otherwise throws an <paramref name="ArgumentException"/>.</returns>
    public Piece Build()
    {
        if(this.movementPattern.Count == 0)
            throw new ArgumentException("Must have at least one movement pattern.");

        string pieceString = Constants.SharedPieceIdentifier;
        if(!this.pc.Equals(PieceClassifier.SHARED))
            pieceString = this.pc == PieceClassifier.WHITE ? whiteCustomPieceIdentifier : blackCustomPieceIdentifier; 

        if(sameCaptureAsMovement)
            return new Piece(this.movementPattern, this.movementPattern, this.royal, this.pc, this.repeat, pieceString, this.canBeCaptured);
        else
            return new Piece(this.movementPattern, this.capturePattern, this.royal, this.pc, this.repeat, pieceString, this.canBeCaptured);
    }

    /// <summary>
    /// Genereates all valid moves from the current state of the builder.
    /// </summary>
    /// <param name="square">is the square where the piece should be inserted on.</param>
    /// <returns>A HashSet of all the curerntly valid moves from the square <paramref name="square"/>.</returns>
    public HashSet<string> GetAllCurrentlyValidMovesFromSquare(string square)
    {
        //The idea behind this method is to be able to continuously draw the currently legal moves in the editor when creating a new piece.
        //Create helper class to remove dependency on MoveWorker and to avoid creating new objects each time this method is called?

        var moveWorker = new MoveWorker(new Chessboard(8));
        var dummyPiece = new Piece(this.movementPattern, this.movementPattern, false, PieceClassifier.WHITE, this.repeat, whiteCustomPieceIdentifier, this.canBeCaptured);
        if(moveWorker.InsertOnBoard(dummyPiece, square))
            return moveWorker.GetAllValidMoves(Player.White);
        throw new ArgumentException("Invalid square for an 8x8 chessboard.");
    }

    public Piece GetDummyPieceWithCurrentMovement()
    {
        return new Piece(this.movementPattern, this.movementPattern, false, PieceClassifier.WHITE, this.repeat, whiteCustomPieceIdentifier, this.canBeCaptured);
    }

    public Piece GetDummyPieceWithCurrentCaptures()
    {
        return new Piece(this.capturePattern, this.capturePattern, false, PieceClassifier.WHITE, this.repeat, whiteCustomPieceIdentifier, this.canBeCaptured);
    }

    /// <summary>
    /// Genereates all valid capture moves from the current state of the builder.
    /// </summary>
    /// <param name="square">is the square where the piece should be inserted on.</param>
    /// <returns>A HashSet of all the curerntly valid capture moves from the square <paramref name="square"/>.</returns>
    public HashSet<string> GetAllCurrentlyValidCaptureMovesFromSquare(string square)
    {
        //The idea behind this method is to be able to continuously draw the currently legal moves in the editor when creating a new piece.
        //Create helper class to remove dependency on MoveWorker and to avoid creating new objects each time this method is called?
        
        if(sameCaptureAsMovement)
            return GetAllCurrentlyValidMovesFromSquare(square);
            
        var moveWorker = new MoveWorker(new Chessboard(8));
        var dummyPiece = new Piece(this.capturePattern, this.capturePattern, false, PieceClassifier.WHITE, this.repeat, whiteCustomPieceIdentifier, this.canBeCaptured);
        if(moveWorker.InsertOnBoard(dummyPiece, square))
            return moveWorker.GetAllValidMoves(Player.White);
        throw new ArgumentException("Invalid square for an 8x8 chessboard.");
    }

#region Add and Remove patterns

    /// <summary>
    /// Adds a regular pattern to the allowed movement patterns. 
    /// </summary>
    /// <param name="xDir">is the direction on the x-axis.</param>
    /// <param name="yDir">is the direction on the y-axis.</param>
    /// <param name="minLength">is the minimum length to be moved in this direction.</param>
    /// <param name="maxLength">is the maximum length to be moved in this direction.</param>
    public void AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this.movementPattern.AddPattern(new RegularPattern(xDir, yDir, minLength, maxLength));
    }

    /// <summary>
    /// Adds a regular pattern to the allowed movement patterns. 
    /// </summary>
    /// <param name="direction">is the direction of the pattern.</param>
    /// <param name="minLength">is the minimum length to be moved in this direction.</param>
    /// <param name="maxLength">is the maximum length to be moved in this direction.</param>
    public void AddMovementPattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        AddMovementPattern(direction.Item1, direction.Item2, minLength, maxLength);
    }

    /// <summary>
    /// Adds a fixed jumping pattern to the allowed patterns.
    /// </summary>
    /// <param name="xOffset">is the offset on the x-axis.</param>
    /// <param name="yOffset">is the offset on the y-axis.</param>
    public void AddJumpMovementPattern(int xOffset, int yOffset)
    {
        this.movementPattern.AddPattern(new JumpPattern(xOffset, yOffset));
    }

    /// <summary>
    /// Removes a regular pattern from the allowed patterns.
    /// </summary>
    /// <param name="xDir">is the direction of the pattern on the x-axis.</param>
    /// <param name="yDir">is the direction of the pattern on the y-axis.</param>
    /// <param name="minLength">is the minimum length of the pattern.</param>
    /// <param name="maxLength">is the maximum length of the pattern.</param>
    public bool RemoveMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        var pattern = new RegularPattern(xDir, yDir, minLength, maxLength);
        return this.movementPattern.RemovePattern(pattern);
    }

    /// <summary>
    /// Removes a regular pattern from the allowed patterns.
    /// </summary>
    /// <param name="direction">is the direction of the pattern.</param>
    /// <param name="minLength">is the minimum length of the pattern.</param>
    /// <param name="maxLength">is the maximum length of the pattern.</param>
    public bool RemoveMovementPattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        return RemoveMovementPattern(direction.Item1, direction.Item2, minLength, maxLength);
    }

    /// <summary>
    /// Removes a fixed jumping pattern from the allowed patterns.
    /// </summary>
    /// <param name="xOffset">is the offset on the x-axis of the pattern.</param>
    /// <param name="yOffset">is the offset on the y-axis of the pattern.</param>
    public bool RemoveJumpMovementPattern(int xOffset, int yOffset)
    {
        var pattern = new JumpPattern(xOffset, yOffset);
        return this.movementPattern.RemovePattern(pattern);
    }

    /// <summary>
    /// Adds a regular pattern to the allowed capture patterns. 
    /// </summary>
    /// <param name="xDir">is the direction on the x-axis.</param>
    /// <param name="yDir">is the direction on the y-axis.</param>
    /// <param name="minLength">is the minimum length to be moved in this direction.</param>
    /// <param name="maxLength">is the maximum length to be moved in this direction.</param>
    public void AddCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this.capturePattern.AddPattern(new RegularPattern(xDir, yDir, minLength, maxLength));
    }

    /// <summary>
    /// Adds a regular pattern to the allowed capture patterns. 
    /// </summary>
    /// <param name="direction">is the direction of the pattern.</param>
    /// <param name="minLength">is the minimum length to be moved in this direction.</param>
    /// <param name="maxLength">is the maximum length to be moved in this direction.</param>
    public void AddCapturePattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        AddCapturePattern(direction.Item1, direction.Item2, minLength, maxLength);
    }

    /// <summary>
    /// Adds a fixed jumping pattern to the allowed capture patterns.
    /// </summary>
    /// <param name="xOffset">is the offset on the x-axis.</param>
    /// <param name="yOffset">is the offset on the y-axis.</param>
    public void AddJumpCapturePattern(int xOffset, int yOffset)
    {
        this.capturePattern.AddPattern(new JumpPattern(xOffset, yOffset));
    }

    /// <summary>
    /// Removes a regular pattern from the allowed capture patterns.
    /// </summary>
    /// <param name="xDir">is the direction of the pattern on the x-axis.</param>
    /// <param name="yDir">is the direction of the pattern on the y-axis.</param>
    /// <param name="minLength">is the minimum length of the pattern.</param>
    /// <param name="maxLength">is the maximum length of the pattern.</param>
    public bool RemoveCapturePattern(int xDir, int yDir, int minLength, int maxLength)
    {
        return this.capturePattern.RemovePattern(new RegularPattern(xDir, yDir, minLength, maxLength));
    }

    /// <summary>
    /// Removes a regular pattern from the allowed capture patterns.
    /// </summary>
    /// <param name="direction">is the direction of the pattern.</param>
    /// <param name="minLength">is the minimum length of the pattern.</param>
    /// <param name="maxLength">is the maximum length of the pattern.</param>
    public bool RemoveCapturePattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        return RemoveCapturePattern(direction.Item1, direction.Item2, minLength, maxLength);
    }
    
    /// <summary>
    /// Removes a fixed jumping pattern from the allowed capture patterns.
    /// </summary>
    /// <param name="xOffset">is the offset on the x-axis of the pattern.</param>
    /// <param name="yOffset">is the offset on the y-axis of the pattern.</param>
    public bool RemoveJumpCapturePattern(int xOffset, int yOffset)
    {
        return this.capturePattern.RemovePattern(new JumpPattern(xOffset, yOffset));
    }

#endregion

    /// <summary>
    /// Set true if the Piece should have the same capture and movement patterns, false to have separate patterns. Is true from the preset.
    /// </summary>
    /// <param name="enable">true to enable same capture and movement pattern, false to disable.</param>
    public void SetSameMovementAndCapturePattern(bool enable) { this.sameCaptureAsMovement = enable; }

    /// <summary>
    /// Set true if the piece can be captured, false if it can not. Is true from the preset.
    /// </summary>
    /// <param name="enable">true to allow piece to be captured, otherwise false.</param>
    public void SetCanBeCaptured(bool enable) { this.canBeCaptured = enable; }

    /// <summary>
    /// Select who the piece belongs to. <paramref name="player"/> can be either "white", "black" or "shared".
    /// Otherwise an <paramref name="ArgumentException"/> is thrown.
    /// </summary>
    /// <param name="player">is the player that the piece belongs to.</param>
    public void BelongsToPlayer(String player) //Chose to not have PieceClassifier as parameter to hide PieceClassifier from client using this builder.
    { 
        PieceClassifier pieceClassifier;
        switch (player)
        {
            case "white"  : pieceClassifier = PieceClassifier.WHITE; break;
            case "black"  : pieceClassifier = PieceClassifier.BLACK; break;
            case "shared" : pieceClassifier = PieceClassifier.SHARED; break;
            default : throw new ArgumentException("Unknown argument of player.");
        }
        this.pc = pieceClassifier;
    }

    /// <summary>
    /// Set how many times the movement pattern can be repeated. Argument must be between 0 and 3.
    /// </summary>
    /// <param name="repeat">is the amount of times the movement pattern should be repeated.</param>
    public void RepeatMovement(int repeat) { this.repeat = repeat; }

    /// <summary>
    /// Set true if the piece should be royal.
    /// </summary>
    /// <param name="enable">true if the piece is royal, otherwise false.</param>
    public void SetRoyal(bool enable) { this.royal = enable; }


}