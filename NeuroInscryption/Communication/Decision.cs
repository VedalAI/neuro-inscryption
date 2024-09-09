using JetBrains.Annotations;
using NeuroInscryption.Communication.Json;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication;

public abstract class Decision
{
    [JsonProperty("game")]
    private string Game => "Inscryption";

    [JsonProperty("state")]
    protected abstract object State { get; }

    [JsonProperty("query")]
    protected abstract string Query { get; }

    [JsonProperty("responseSchema")]
    protected abstract JsonSchema ResponseSchema { get; }
}
