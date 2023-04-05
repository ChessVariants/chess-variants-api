using ChessVariantsAPI.Authentication;
using ChessVariantsAPI.DTOs;
using DataAccess.MongoDB;
using DataAccess.MongoDB.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ChessVariantsAPI.Controllers;

/// <summary>
/// This controller exposes endpoints for handling variants, i.e creating a new variant
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class VariantsController : GenericController
{
    public VariantsController(DatabaseService databaseService, ILogger<VariantsController> logger) : base(databaseService, logger)
    {
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Variant>>> Get()
    {
        var variants = await _db.Variants.GetAsync();
        _logger.LogInformation("Get request found {amount} variants", variants.Count);
        return Ok(variants);
    }

    [HttpPost]
    public async Task<ActionResult<CreatedVariantDTO>> CreateVariant(CreateVariantDTO createVariantDTO)
    {
        _logger.LogInformation("Creating variant: {variant}", createVariantDTO);
        
        var variant = new Variant
        {
            Id = createVariantDTO.Id,
            Creator = createVariantDTO.Creator,
            Description = createVariantDTO.Description,
            VariantData = createVariantDTO.VariantData,
        };

        try
        {
            await _db.Variants.CreateAsync(variant);
        }
        catch (MongoException e)
        {
            _logger.LogError("Writing variant {u} error: {e}", createVariantDTO, e.Message);
            return BadRequest("An Unexpected error occured");
        }

        createVariantDTO.Password = "";
        return CreatedAtAction("Get", new CreatedUserDTO { Email = createUserDTO.Email, Username = createUserDTO.Email });
    }
}
