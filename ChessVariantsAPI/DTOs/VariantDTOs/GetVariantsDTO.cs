using System.ComponentModel.DataAnnotations;

namespace ChessVariantsAPI.DTOs.VariantDTOs;

public record GetVariantsDTO
{
    public List<VariantInfoDTO> VariantInfo { get; set; } = null!;

    public int VariantAmount = 0;
}

public record VariantInfoDTO
{
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public string Code { get; set; } = null!;

    public string Creator { get; set; } = null!;
}
