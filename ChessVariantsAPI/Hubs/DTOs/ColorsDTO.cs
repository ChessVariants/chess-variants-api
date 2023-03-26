using Newtonsoft.Json;

namespace ChessVariantsAPI.Hubs;

public record ColorsDTO
{
    [JsonProperty("white")]
    public string? White { get; set; }

    [JsonProperty("black")]
    public string? Black { get; set; }

    public string AsJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}