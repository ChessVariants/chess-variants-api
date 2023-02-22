namespace ChessVariantsLogic;

public class Piece
{
    private readonly List<Tuple<int, int>> movement;
    private bool jump;
    private (int, int) moveLength;
    private int repeat;
    private bool royal;
    private PieceClassifier pieceClassifier;
    private bool hasMoved;

    public Piece(List<Tuple<int, int>> movement, bool jump, (int,int) moveLength, int repeat, bool royal, bool hasMoved, PieceClassifier pc)
    {
        this.movement = movement;
        this.jump = jump;
        this.moveLength = moveLength;
        this.repeat = repeat;
        this.royal = royal;
        this.hasMoved = hasMoved;
        this.pieceClassifier = pc;
    }


}