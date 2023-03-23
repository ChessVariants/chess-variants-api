namespace ChessVariantsLogic;

public class PieceBuilder
{
    private MovementPattern movementPattern;
    private MovementPattern capturePattern;
    private bool royal;
    private PieceClassifier? pc;
    private int repeat;
    private bool canBeCaptured;

    private bool sameCaptureAsMovement;

    public PieceBuilder()
    {
        this.movementPattern = new MovementPattern();
        this.capturePattern = new MovementPattern();
        this.royal = false;
        this.pc = null;
        this.repeat = 0;
        this.canBeCaptured = true;
        this.sameCaptureAsMovement = true;
    }

    public void Reset()
    {
        this.movementPattern = new MovementPattern();
        this.capturePattern = new MovementPattern();
        this.royal = false;
        this.pc = null;
        this.repeat = 0;
        this.canBeCaptured = true;
        this.sameCaptureAsMovement = true;
    }

    //TODO: Checks that no argument is null
    //TODO: Dynamic solution to the string pieceIdentifier
    public Piece Build()
    {
        if(this.movementPattern.Count == 0)
            throw new ArgumentException("Must have at least one movement pattern.");
        if(this.pc == null)
            throw new ArgumentException("Piece must belong to a player.");

        var pieceClassifier = (PieceClassifier) this.pc;

        string pi = Constants.SharedPieceIdentifier;
        if(!this.pc.Equals(PieceClassifier.SHARED))
            pi = this.pc == PieceClassifier.WHITE ? "CA" : "ca"; 

        if(sameCaptureAsMovement)
            return new Piece(this.movementPattern, this.movementPattern, this.royal, pieceClassifier, this.repeat, pi, this.canBeCaptured);
        else
            return new Piece(this.movementPattern, this.capturePattern, this.royal, pieceClassifier, this.repeat, pi, this.canBeCaptured);
    }

    public void AddMovementPattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        this.movementPattern.AddPattern(new RegularPattern(direction, minLength, maxLength));
    }

    public void AddMovementPattern(int xOffset, int yOffset)
    {
        this.movementPattern.AddPattern(new JumpPattern(xOffset, yOffset));
    }

    public void RemoveMovementPattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        var pattern = new RegularPattern(direction, minLength, maxLength);
        this.movementPattern.RemovePattern(pattern);
    }

    public void AddCapturePattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        this.capturePattern.AddPattern(new RegularPattern(direction, minLength, maxLength));
    }

    public void AddCapturePattern(int xOffset, int yOffset)
    {
        this.capturePattern.AddPattern(new JumpPattern(xOffset, yOffset));
    }

    public void RemoveCapturePattern(Tuple<int,int> direction, int minLength, int maxLength)
    {
        this.capturePattern.RemovePattern(new RegularPattern(direction, minLength, maxLength));
    }
    
    public void RemoveCapturePattern(int xOffset, int yOffset)
    {
        this.capturePattern.RemovePattern(new JumpPattern(xOffset, yOffset));
    }

    public void SetSameMovementAndCapturePattern(bool enable)
    {
        this.sameCaptureAsMovement = enable;
    }

    public void SetCanBeCaptured(bool enable)
    {
        this.canBeCaptured = enable;
    }

    public void BelongsToPlayer(PieceClassifier pieceClassifier)
    {
        this.pc = pieceClassifier;
    }

    public void RepeatMovement(int repeat)
    {
        this.repeat = repeat;
    }

    public void SetRoyal(bool enable)
    {
        this.royal = enable;
    }


}