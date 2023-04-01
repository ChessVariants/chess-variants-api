using Newtonsoft.Json;

namespace ChessVariantsAPI.Hubs.DTOs;

public record JoinResultDTO
{
    [JsonProperty("color")]
    public string? Color { get; set; }

    [JsonProperty("success")]
    public bool? Success { get; set; } = false;

    [JsonProperty("failReason")]
    public string? FailReason { get; set; }

    public string AsJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
