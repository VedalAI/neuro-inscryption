using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class DialogueEvents
{
    static DialogueEvents()
    {
    }

    private static readonly Dictionary<string, ReactionTracker> _events = [];

    private static void AddEvent(string dialogue, ReactionTracker reaction)
    {
        _events[dialogue.Trim().ToLowerInvariant()] = reaction;
    }

    [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowMessage))]
    [HarmonyPrefix]
    private static void HandleLine(string message)
    {
        if (_events.TryGetValue(message.Trim().ToLowerInvariant(), out ReactionTracker? reaction))
        {
            reaction.Send();
        }
    }
}
