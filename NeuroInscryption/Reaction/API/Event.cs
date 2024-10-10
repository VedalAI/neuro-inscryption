using System;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Tweaks;

namespace NeuroInscryption.Reaction.API;

public class Event(string context, Priority priority)
{
    public static implicit operator Event((string context, Priority priority) tuple) => new(tuple.context, tuple.priority);

    public static Priority? OverridePriority;

    private const string OverridePrioCommand = "prio";
    static Event()
    {
        TwitchIntegration.HandleTwitchMessage += HandleTwitchMessage;
    }

    public static void HandleTwitchMessage(string message)
    {
        if (message.Split(' ') is [OverridePrioCommand, string prio])
        {
            if (prio is "clear")
                OverridePriority = null;
            else if (Enum.TryParse(prio, ignoreCase: true, out Priority @override))
                OverridePriority = @override;
        }
    }

    public void Send(params object[] format)
    {
        Priority prioUsed = OverridePriority ?? priority;
        new Context(string.Format(context, format), prioUsed == Priority.Silent, prioUsed == Priority.Force).Send();
    }
}
