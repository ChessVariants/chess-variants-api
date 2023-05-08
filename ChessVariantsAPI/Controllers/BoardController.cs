using ChessVariantsAPI.DTOs.VariantDTOs;
using DataAccess.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChessVariantsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BoardController : GenericController
{
    public BoardController(DatabaseService databaseService) : base(databaseService)
    {
    }

    [HttpGet]
    public async Task<ActionResult<BoardOptionsDTO>> GetBoards()
    {
        var username = GetUsername();
        var boardsBelongingToUser = await _db.Chessboards.GetByUserAsync(username);
        var boardDTOList = boardsBelongingToUser.Select(board => new BoardDTO
        {
            Name = board.Name,
        }).ToList();
        return Ok(new BoardOptionsDTO { BoardOptions = boardDTOList});
    }

    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }
}
