using ChessVariantsAPI.ObjectTranslations;
using ChessVariantsLogic;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Mvc;

namespace ChessVariantsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PieceController : GenericController
{
    public PieceController(DatabaseService databaseService, ILogger<UsersController> logger) : base(databaseService, logger)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetPiece()
    {
        var logicPiece = Piece.Queen(PieceClassifier.WHITE);
        var modelPiece = PieceTranslator.CreatePieceModel(logicPiece, "test-queen", "me", "images/queen.svg");
        await _db.Pieces.CreateAsync(modelPiece);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SavePiece(DataAccess.MongoDB.Models.Piece piece)
    {
        piece.Id = null;
        await _db.Pieces.CreateAsync(piece);
        return Ok();
    }
}
