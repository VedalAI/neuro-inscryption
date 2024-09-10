using NeuroInscryption.Communication.Json;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication;

public sealed class Decision
{
    [JsonProperty("game")]
    private string Game => "Inscryption";

    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("state")]
    public required object State { get; set; }

    [JsonProperty("query")]
    public required string Query { get; set; }

    [JsonProperty("responseSchema")]
    public required JsonSchema ResponseSchema { get; set; }
}
