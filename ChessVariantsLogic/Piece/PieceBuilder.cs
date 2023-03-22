namespace ChessVariantsLogic;

public class PieceBuilder
{
    private MovementPattern movementPattern;
    private MovementPattern capturePattern;
    private bool royal;
    private PieceClassifier pc;
    private int repeat;
    private bool canBeCaptured;

    public PieceBuilder()
    {
        this.movementPattern = new MovementPattern();
        this.capturePattern = new MovementPattern();
        this.royal = false;
        this.repeat = 0;
        this.canBeCaptured = true;
    }

    public void Reset()
    {
        this.movementPattern = new MovementPattern();
        this.capturePattern = new MovementPattern();
        this.royal = false;
        this.repeat = 0;
        this.canBeCaptured = true;
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

    public Piece Build()
    {
        //TODO: Checks that no argument is null
        //TODO: Dynamic solution to the string pieceIdentifier
        return new Piece(this.movementPattern, this.capturePattern, this.royal, this.pc, false, this.repeat, "CA", this.canBeCaptured);
    }


}