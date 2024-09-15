using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Messages;

public interface ICustomIncomingMessageHandler
{
    void SetJsonData(JToken? data);
}
