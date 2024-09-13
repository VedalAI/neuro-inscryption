using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Messages;

public interface ICustomIncomingMessageHandler
{
    bool SetJsonData(JToken? data);
}
