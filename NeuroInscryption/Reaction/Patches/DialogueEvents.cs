using System;
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
    }

    private static readonly Dictionary<string, ReactionTracker> _events = new(StringComparer.OrdinalIgnoreCase)
    {
        ["You done gawking?"] = Reactions.Act3Starting,
        ["You're not used to seeing <color=#D93846>5</color> lanes are you?"] = Reactions.FiveLanes,
        ["We have gained control of the robot!"] = Reactions.MycologistsFound,
        ["So that's what a <color=#69FFA0>mole</color> really looks like..."] = Reactions.G0llyMoleImage,
        ["Are these... your <color=#69FFA0>friends</color>?"] = Reactions.G0llyFriendsStart,
        ["Make a card for someone else?"] = Reactions.G0llyCreateCard,
        ["No... We don't think so."] = Reactions.HiddenScrybes,
        ["Right <color=#D93846>Luke</color>?"] = Reactions.LukeAppears,
        ["Oh, the fun we would have had together!"] = Reactions.MeetingGrimora, // grimora chess board
        //["Oh! Do you think me selfish?"] = Reactions.MeetingGrimora, // first grimora card battle
        //["Shall we play one more game?"] = Reactions.MeetingLeshy,
        ["It was a good deck. You did well."] = Reactions.MeetingLeshy, // better place to make a comment
        ["Spare me and whatever is left?"] = Reactions.MeetingMagnificus,
    };

    private static void AddEvent(string dialogue, ReactionTracker reaction)
    {
        _events[dialogue.Trim().ToLowerInvariant()] = reaction;
    }

    [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowMessage))]
    [HarmonyPrefix]
    private static void HandleLine(string message)
    {
        Plugin.Logger.LogDebug(message);
        if (_events.TryGetValue(message.Trim(), out ReactionTracker? reaction))
        {
            reaction.Send();
        }
    }
}
