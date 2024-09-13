using System.Diagnostics.CodeAnalysis;
using NeuroInscryption.Communication.Actions;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Messages.Incoming;

public class Action : ICustomIncomingMessageHandler
{
    public string? ActionName { get; private set; }
    public JToken? ActionData { get; private set; }

    bool ICustomIncomingMessageHandler.SetJsonData(JToken? data)
    {
        if (data == null) return false;

        ActionName = data["action"]?.Value<string>();
        ActionData = data["data"];

        return !string.IsNullOrWhiteSpace(ActionName);
    }

    public bool Match<TResponse>(IAction<TResponse> iAction, [NotNullWhen(true)] out TResponse? response)
    {
        if (iAction.ActionName != ActionName)
        {
            response = default;
            return false;
        }

        return iAction.Deserialize(this, out response);
    }
}
