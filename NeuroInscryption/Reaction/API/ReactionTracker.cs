using System;

namespace NeuroInscryption.Reaction.API;

public sealed class ReactionTracker(Func<uint, Event?> getterFunc)
{
    private uint _occ;

    public ReactionTracker(Event evnt) : this(_ => evnt)
    {
    }

    public void Send()
    {
        getterFunc(_occ++)?.Send();
    }
}