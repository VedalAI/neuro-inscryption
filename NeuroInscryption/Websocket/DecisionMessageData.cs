using Newtonsoft.Json;

namespace NeuroInscryption.Websocket;

internal sealed class DecisionMessageData : IMessageData
{
    [JsonProperty("game")]
    private string Game => "Inscryption";
}
