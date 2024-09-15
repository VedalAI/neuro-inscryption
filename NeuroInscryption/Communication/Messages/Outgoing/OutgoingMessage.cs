using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Communication.Websocket;

namespace NeuroInscryption.Communication.Messages.Outgoing;

public abstract class OutgoingMessage<TResponse> where TResponse : class, new()
{
    protected abstract WsMessage GetMessage();

    protected abstract string ResponseCommand { get; }

    private Response<TResponse> Send()
    {
        WebsocketConnection.Send(GetMessage());
        Response<TResponse> response = new();
        response.SubscribeTo(ResponseCommand);
        return response;
    }

    public async UniTask<TResponse> SendAsync(CancellationToken ct = default)
    {
        Response<TResponse> response = Send();
        await response.WithCancellation(ct);
        return response.Result!;
    }
}
