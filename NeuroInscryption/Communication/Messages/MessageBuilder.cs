using NeuroInscryption.Communication.Websocket;

namespace NeuroInscryption.Communication.Messages;

public abstract class MessageBuilder<T> where T : class, new()
{
    protected abstract WsMessage GetMessage();

    protected abstract string ResponseCommand { get; }

    public Response<T> Send()
    {
        WebsocketConnection.Send(GetMessage());
        Response<T> response = new();
        response.SubscribeTo(ResponseCommand);
        return response;
    }
}
