using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Messages.Incoming;

public interface ICustomIncomingMessageHandler
{
    void SetJsonData(JToken? data);
}
