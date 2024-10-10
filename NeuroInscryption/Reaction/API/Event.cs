using NeuroInscryption.Communication.Messages.Outgoing;

namespace NeuroInscryption.Reaction.API;

public class Event(string context, Priority priority)
{
    public static implicit operator Event((string context, Priority priority) tuple) => new(tuple.context, tuple.priority);

    public void Send(params object[] format)
    {
        new Context(string.Format(context, format), priority == Priority.Silent, priority == Priority.Force).Send();
    }
}
