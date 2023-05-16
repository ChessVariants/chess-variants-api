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
            CanBePromotedTo = piece.CanBePromotedTo,
            ImagePath = imagePath,
            BelongsTo = piece.PieceClassifier.AsString(),
            Movement = TranslatePatterns(piece.GetAllMovementPatterns()),
            Captures = TranslatePatterns(piece.GetAllCapturePatterns()),
        };
    }

    public static ChessVariantsLogic.Piece CreatePieceLogic(DataAccess.MongoDB.Models.Piece pieceModel)
    {
        PieceClassifier pc = PieceClassifier.SHARED;
        switch (pieceModel.BelongsTo)
        {
            case "white" : pc = PieceClassifier.WHITE; break;
            case "black" : pc = PieceClassifier.BLACK; break;
        }
        return new ChessVariantsLogic.Piece
        (
            TranslateToLogicPatterns(pieceModel.Movement),
            TranslateToLogicPatterns(pieceModel.Captures),
            pc,
            pieceModel.Repeat,
            pieceModel.Name,
            pieceModel.CanBeCaptured,
            pieceModel.CanBePromotedTo,
            pieceModel.ImagePath
        );
    }

    private static ChessVariantsLogic.MovementPattern TranslateToLogicPatterns(List<MovePattern> modelPatterns)
    {
        var logicPatterns = new HashSet<Pattern>();
        foreach (var mp in modelPatterns)
        {
            if (mp.MinLength == -1)
                logicPatterns.Add(new JumpPattern(mp.XDir, mp.YDir));
            else
                logicPatterns.Add(new RegularPattern(mp.XDir, mp.YDir, mp.MinLength, mp.MaxLength));
        }
        return new MovementPattern(logicPatterns);
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
