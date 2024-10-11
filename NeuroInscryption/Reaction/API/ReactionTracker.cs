using System;

namespace NeuroInscryption.Reaction.API;

public sealed class ReactionTracker(Func<uint, Event?> getterFunc)
{
    private uint _occ;

    public ReactionTracker(Event evnt, bool once = false) : this(num => !once || num == 0 ? evnt : null)
    {
    }

    public ReactionTracker(string context, Priority priority, bool once = false) : this(new Event(context, priority), once)
    {
    }

    public void Send(params object[] format)
    {
        try
        {
            getterFunc(_occ++)?.Send(format);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
        }
    }
}
