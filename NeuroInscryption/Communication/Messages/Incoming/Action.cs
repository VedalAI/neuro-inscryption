using System;
using System.Diagnostics.CodeAnalysis;
using NeuroInscryption.Communication.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication.Messages.Incoming;

public class Action : ICustomIncomingMessageHandler
{
    public string? ActionName { get; private set; }
    public JToken? ActionData { get; private set; }

    void ICustomIncomingMessageHandler.SetJsonData(JToken? data)
    {
        if (data == null) throw new JsonReaderException("Received invalid JSON data for action message");

        ActionName = data["action"]?.Value<string>();
        ActionData = data["data"];
    }

    public bool Match(IAction iAction)
    {
        return iAction.ActionName == ActionName;
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

    [DoesNotReturn]
    public void ThrowInvalid()
    {
        throw new InvalidOperationException($"Received action '{ActionName}' which did not match possible expected actions.");
    }
}
