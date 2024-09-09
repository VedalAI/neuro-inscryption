namespace NeuroInscryption.Websocket;

public interface IMessageData
{

    public static implicit operator IMessageData(string data)
    {
        return new MessageData(data);
    }
}
