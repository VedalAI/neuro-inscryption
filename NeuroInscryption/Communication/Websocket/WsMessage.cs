using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Websocket;

public record class WsMessage
{
    [JsonProperty("command")]
    public readonly string Command;

    [JsonProperty("data")]
    public readonly object Data;

    public WsMessage(string Command, object Data)
    {
        this.Command = Command;
        this.Data = Data;
    }

    public string SerializeToJson()
    {
        return JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
        });
    }
}
