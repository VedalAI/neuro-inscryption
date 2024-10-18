using System.Diagnostics.CodeAnalysis;
using NeuroInscryption.Communication.Websocket;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Messages.Outgoing;

[method: SetsRequiredMembers]
public sealed class Context(string message, bool silent, bool force) : OutgoingMessage
{
    [JsonProperty("game", Order = 0)]
    private string Game => "Inscryption";

    [JsonProperty("message", Order = 10)]
    public required string Message { get; set; } = message;

    [JsonProperty("silent", Order = 20)]
    public required bool Silent { get; set; } = silent;

    [JsonProperty("force", Order = 30)]
    public required bool Force { get; set; } = force;

    protected override WsMessage GetMessage() => new("context", this);
}