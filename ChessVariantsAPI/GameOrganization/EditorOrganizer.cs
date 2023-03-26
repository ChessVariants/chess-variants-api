using ChessVariantsLogic;

namespace ChessVariantsAPI;

public class EditorOrganizer
{
    private readonly Editor _editor;

    public EditorOrganizer()
    {
        this._editor = new Editor();
    }

    public EditorEvent AddMovementPattern(int xDir, int yDir, int minLength, int maxLength)
    {
        return this._editor.AddMovementPattern(xDir, yDir, minLength, maxLength);
    }

    public string GetStateAsJson()
    {
        return this._editor.ExportStateAsJson();
    }


}