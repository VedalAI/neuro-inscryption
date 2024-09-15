using System.Diagnostics.CodeAnalysis;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Websocket;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Messages.Outgoing;

[method: SetsRequiredMembers]
public sealed class Decision(string id, object state, string query) : OutgoingMessage<Action>
{
    [JsonProperty("game")]
    private string Game => "Inscryption";

    [JsonProperty("id")]
    public required string Id { get; set; } = id;

    [JsonProperty("state")]
    public required object State { get; set; } = state;

    [JsonProperty("query")]
    public required string Query { get; set; } = query;

    [JsonProperty("responseSchema")]
    private readonly JsonSchema _responseSchema = JsonSchema.NewResponseSchema();

    protected override WsMessage GetMessage() => new("decision", this);
    protected override string ResponseCommand => "action";

    public void AddAction<TResponse>(IAction<TResponse> action)
    {
        _responseSchema.Apply(action);
    }
}
