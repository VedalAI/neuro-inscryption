using System.Collections.Generic;
using Newtonsoft.Json;

namespace NeuroInscryption.Websocket;

internal abstract class WsMessage(string command)
{
    [JsonProperty("command")]
    public string Command { get; } = command;

    [JsonProperty("data")]
    public abstract IMessageData Data { get; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static implicit operator string(WsMessage wsMessage)
    {
        return wsMessage.ToString();
    }
}
