using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using NeuroInscryption.Communication.Websocket;
using NeuroInscryption.Tweaks;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Messages.Outgoing;

[method: SetsRequiredMembers]
public sealed class RequestMute(bool muted) : OutgoingMessage
{
    [JsonProperty("game", Order = 0)]
    private string Game => "Inscryption";

    static RequestMute()
    {
        TwitchIntegration.HandleTwitchMessage += HandleTwitchMessage;
    }

    private static void HandleTwitchMessage(string message)
    {
        bool? muted = message switch
        {
            "mute" => true,
            "unmute" => false,
            _ => null
        };
        if (muted.HasValue)
        {
            new RequestMute(muted.Value).Send();
            Plugin.Logger.LogMessage($"Twitch Backdoor - Setting mute state to {muted.Value}");
        }
    }

    protected override WsMessage GetMessage() => new(muted ? "mute" : "unmute", this);
}
