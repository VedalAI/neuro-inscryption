using System.Diagnostics.CodeAnalysis;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Websocket;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Messages.Outgoing;

[method: SetsRequiredMembers]
public sealed class Decision(string id, IGameState state, string query) : OutgoingMessage<Action>
{
    [JsonProperty("game", Order = 0)]
    private string Game => "Inscryption";

    [JsonProperty("id", Order = 20)]
    public required string Id { get; set; } = id;

    [JsonProperty("state", Order = 10)]
    public required object State { get; set; } = state;

    [JsonProperty("query", Order = 30)]
    public required string Query { get; set; } = query;

    [JsonProperty("responseSchema", Order = 40)]
    private readonly JsonSchema _responseSchema = JsonSchema.NewResponseSchema();

    [SetsRequiredMembers]
    public Decision(Description decisionDescription, IGameState state) : this(decisionDescription.Name, state, decisionDescription.Info)
    {
    }

    protected override WsMessage GetMessage()
    {
        _responseSchema.Prepare();
        return new("decision", this);
    }

    protected override string ResponseCommand => "action";

    public void AddAction(IAction action)
    {
        _responseSchema.Apply(action);
    }
}
