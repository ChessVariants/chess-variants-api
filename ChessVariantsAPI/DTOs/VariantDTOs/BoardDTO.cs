namespace ChessVariantsAPI.DTOs.VariantDTOs;

public record BoardDTO
{
    public string Name { get; set;} = null!;
}

public record BoardOptionsDTO
{
    public List<BoardDTO> BoardOptions { get; set; } = null!;
}
