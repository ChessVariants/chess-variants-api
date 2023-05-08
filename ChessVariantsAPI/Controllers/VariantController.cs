using ChessVariantsAPI.DTOs.VariantDTOs;
using DataAccess.MongoDB;
using DataAccess.MongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ChessVariantsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VariantController : GenericController
{
    public VariantController(DatabaseService databaseService) : base(databaseService)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetVariants()
    {
        var username = GetUsername();

        var variantsBelongingToUser = await _db.Variants.FindAsync(variant => variant.Creator == username);
        var variantInfoList = variantsBelongingToUser.Select(variant => new VariantInfoDTO
        {
            Name = variant.Name,
            Code = variant.Code,
            Description = variant.Description,
            Creator = variant.Creator,
        }).ToList();
        var GetVariantsDTO = new GetVariantsDTO { VariantInfo = variantInfoList, VariantAmount = variantInfoList.Count };
        return Ok(GetVariantsDTO);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVariant(CreateVariantDTO createVariantDTO)
    {
        var username = GetUsername();


        var variant = new Variant
        {
            Name = createVariantDTO.Name,
            Creator = username,
            Code = GenerateCode(),
            Description = createVariantDTO.Description,
            WhiteRuleSetIdentifier = createVariantDTO.WhiteRuleSetIdentifier,
            BlackRuleSetIdentifier = createVariantDTO.BlackRuleSetIdentifier,
            BoardIdentifier = createVariantDTO.BoardIdentifier,
            MovesPerTurn = createVariantDTO.MovesPerTurn,
        };

        await _db.Variants.CreateAsync(variant);
        return Ok();
    }

    private string GetUsername()
    {
        return User?.FindFirst(ClaimTypes.Name)?.Value!;
    }

    private static string GenerateCode()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(7));
    }
}
