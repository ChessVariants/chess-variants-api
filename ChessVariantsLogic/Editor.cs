namespace ChessVariantsLogic;

public class Editor
{

    private PieceBuilder builder;

    public Editor()
    {
        this.builder = new PieceBuilder();
    }

    public EditorEvent AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this.builder.AddMovementPattern(xDir, yDir, minLength, maxLength);

        return EditorEvent.Success;
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
}