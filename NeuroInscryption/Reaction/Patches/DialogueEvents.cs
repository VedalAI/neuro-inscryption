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
        // Note: Most of these dialogue lines only happen on the first occurrence of the event.

        AddEvent("You noticed one of the survivors wiping drool from their mouth.", Reactions.Campfire);
        AddEvent("Heeeeeeee-haaw! 'Twas <color=#D93846>The Prospector</color>!", Reactions.ProspectorAppeared);
        AddEvent("G-G-GOLD! I'VE STRUCK GOLD!", Reactions.ProspectorUsedAbility);
        AddEvent("I am <color=#D93846>The Angler</color>. Go fish.", Reactions.AnglerAppeared);
        AddEvent("Sorry. But today <color=#D93846>you</color> will supply the pelts.", Reactions.TrapperAppeared);
        AddEvent("Let's trade.", Reactions.TrapperSwappedToTrader);
        AddEvent("<color=#D93846>Too fast. Too soon.</color>", Reactions.GrizzlyWallSpawned);
        AddEvent("You took your time out there.", Reactions.LeshyRevealed);
        AddEvent("Now <color=#D93846>that</color> is value.", Reactions.LeshyPlayedMoon);
        AddEvent("<color=#D93846>Starvation</color> began to set in...", Reactions.Starvation);
        AddEvent("Surely this food will please you.", Reactions.LeshyCake);
        AddEvent("The last challenger managed to defeat me.", Reactions.LeshyCommentedOnPreviousRun);
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
        Plugin.Logger.LogWarning(message);

        if (_events.TryGetValue(message.Trim().ToLowerInvariant(), out ReactionTracker? reaction))
        {
            reaction.Send();
        }
    }
}
