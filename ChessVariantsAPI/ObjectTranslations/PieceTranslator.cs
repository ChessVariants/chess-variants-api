using ChessVariantsLogic;
using DataAccess.MongoDB.Models;

namespace ChessVariantsAPI.ObjectTranslations;

public static class PieceTranslator
{
    public static DataAccess.MongoDB.Models.Piece CreatePieceModel(ChessVariantsLogic.Piece piece, string name, string creator, string imagePath)
    {
        return new DataAccess.MongoDB.Models.Piece
        {
            Name = name,
            Creator = creator,
            Repeat = piece.Repeat,
            CanBeCaptured = piece.CanBeCaptured,
            ImagePath = imagePath,
            Movement = TranslatePatterns(piece.GetAllMovementPatterns()),
            Captures = TranslatePatterns(piece.GetAllCapturePatterns()),
        };
    }

    private static List<MovePattern> TranslatePatterns(IEnumerable<Pattern> logicPatterns)
    {
        var ModelMovePatterns = new List<MovePattern>();
        foreach (var pattern in logicPatterns)
        {
            MovePattern m = new()
            {
                XDir = pattern.XDir,
                YDir = pattern.YDir,
                MaxLength = pattern.MaxLength,
                MinLength = pattern.MinLength
            };
            ModelMovePatterns.Add(m);
        }
        return ModelMovePatterns;
    }
}
