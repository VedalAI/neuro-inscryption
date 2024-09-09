namespace NeuroInscryption.Websocket;

internal class DecisionMessage() : WsMessage("decision")
{
    public override IMessageData Data { get; }
}
