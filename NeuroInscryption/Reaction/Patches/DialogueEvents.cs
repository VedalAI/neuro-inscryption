using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;
using NeuroInscryption.Reaction.Strings;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class DialogueEvents
{
    static DialogueEvents()
    {
        // Note: Most of these dialogue lines only happen on the first occurrence of the event.

        AddEvent("You noticed one of the survivors wiping drool from their mouth.", Reactions.Campfire);
        AddEvent("Sorry. But today <color=#D93846>you</color> will supply the pelts.", Reactions.TrapperAppeared);
        AddEvent("Let's trade.", Reactions.TrapperSwappedToTrader);
        AddEvent("You took your time out there.", Reactions.LeshyRevealed);
        AddEvent("Now <color=#D93846>that</color> is value.", Reactions.LeshyPlayedMoon);
        AddEvent("I do not celebrate often and I seldom give gifts.", Reactions.LeshyCake);
        AddEvent("The last challenger managed to defeat me.", Reactions.LeshyCommentedOnPreviousRun);
        AddEvent("Oh... oh no. You can't use that.", Reactions.GooBottleAppeared);
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
