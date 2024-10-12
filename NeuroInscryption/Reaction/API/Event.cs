using System;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Tweaks;

namespace NeuroInscryption.Reaction.API;

public sealed class Event(string context, Priority priority)
{
    public static implicit operator Event((string context, Priority priority) tuple) => new(tuple.context, tuple.priority);

    private static Priority _maxPriority = Priority.Force;

    private const string OverrideMaxPrioCommand = "maxprio";

    static Event()
    {
        TwitchIntegration.HandleTwitchMessage += HandleTwitchMessage;
    }

    private static void HandleTwitchMessage(string message)
    {
        if (message.Split(' ') is [OverrideMaxPrioCommand, string prio])
        {
            if (Enum.TryParse(prio, ignoreCase: true, out Priority @override))
            {
                _maxPriority = @override;
                Plugin.Logger.LogMessage($"Twitch Backdoor - Setting max priority to {_maxPriority}");
            }
        }
    }

    public void Send(params object[] format)
    {
        Priority prioUsed = _maxPriority < priority ? _maxPriority : priority;
        if (prioUsed == Priority.Disable) return;

        new Context(string.Format(context, format), prioUsed == Priority.Silent, prioUsed == Priority.Force).Send();
    }
}
