using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Websocket;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Messages.Outgoing;

public sealed class Decision : MessageBuilder<Action>
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

    protected override WsMessage GetMessage() => new("decision", this);
    protected override string ResponseCommand => "action";
}
