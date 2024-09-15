using NeuroInscryption.Communication.Messages;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NeuroInscryption.Communication.Websocket;

public sealed class Response<T> : CustomYieldInstruction where T : class, new()
{
    public void SubscribeTo(string expectedCommand)
    {
        WebsocketConnection.SubscribeOnce(expectedCommand, OnMessageReceived);
    }

    private void OnMessageReceived(JToken? messageData)
    {
        if (typeof(ICustomIncomingMessageHandler).IsAssignableFrom(typeof(T)))
        {
            T result = new();
            bool success = ((ICustomIncomingMessageHandler) result).SetJsonData(messageData);
            if (success) Result = result;
        }
        else
        {
            Result = messageData?.ToObject<T>();
        }

        _isDone = true;
    }

    public T? Result { get; private set; }
    private bool _isDone;

    public override bool keepWaiting => !_isDone;
}
