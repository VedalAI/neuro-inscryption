using Newtonsoft.Json;

namespace NeuroInscryption.Communication;

public class DecisionResponse
{
    [JsonProperty("reasoning")]
    public string Reasoning { get; set; }

    [JsonProperty("decision")]
    public string Decision { get; set; }

    [JsonProperty("data")]
    public object Data { get; set; }
}
