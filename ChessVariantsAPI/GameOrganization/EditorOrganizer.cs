using ChessVariantsLogic.Editor;

namespace ChessVariantsAPI;

public class EditorOrganizer
{
    private readonly PieceEditor _pieceEditor;

    public EditorOrganizer()
    {
        this._pieceEditor = new PieceEditor();
    }

    public void AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        this._pieceEditor.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public string GetStateAsJson()
    {
        return this._pieceEditor.ExportPieceAsJson();
    }


}