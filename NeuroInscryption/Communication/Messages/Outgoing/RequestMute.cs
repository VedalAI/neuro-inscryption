using System.Diagnostics.CodeAnalysis;
using NeuroInscryption.Communication.Websocket;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Messages.Outgoing;

[method: SetsRequiredMembers]
public sealed class RequestMute(bool muted) : OutgoingMessage
{
    [JsonProperty("game", Order = 0)]
    private string Game => "Inscryption";

    protected override WsMessage GetMessage() => new(muted ? "mute" : "unmute", this);
}
