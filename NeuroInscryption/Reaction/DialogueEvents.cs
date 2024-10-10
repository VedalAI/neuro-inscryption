using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Reaction.API;
using Priority = NeuroInscryption.Reaction.API.Priority;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
internal static class DialogueEvents
{
    static DialogueEvents()
    {
        // Note: Most of these dialogue lines only happen on the first occurrence of the event.

        AddEvent("You noticed one of the survivors wiping drool from their mouth.", new ReactionTracker("The survivors of the campfire offer shelter to a creature. They claim that the chosen creature will gain a buff as a result, however, these survivors look famished and untrustworthy.", Priority.React, true));

        AddEvent("Heeeeeeee-haaw! 'Twas <color=#D93846>The Prospector</color>!", new ReactionTracker("A hobbled figure appeared... the first boss encounter: 'Heeeeeeee-haaw! 'Twas The Prospector!'. His mule seems very dear to him.", Priority.React, true));

        AddEvent("G-G-GOLD! I'VE STRUCK GOLD!", new ReactionTracker(num => num switch
        {
            0 => ("The Prospector just struck all played cards, killing them and replacing them with useless golden nuggets! He is breaking the rules of the game!", Priority.React),
            1 => ("The Prospector replaced all cards with gold nuggets again.", Priority.Silent),
            _ => null
        }));

        AddEvent("I am <color=#D93846>The Angler</color>. Go fish.", new ReactionTracker("A hulking man sat by a brackish pond with his feet submerged in the dark water. He pulled a hook from a pile of rotting fish and rose to his feet. The second boss encounter is the Angler. Go fish.", Priority.React, true));

        AddEvent("Sorry. But today <color=#D93846>you</color> will supply the pelts.", new ReactionTracker("You took in the familiar sights and scents of The Trapper's pelts. However, the once friendly man now beheld you with a steelzy gaze. The third boss encounter is starting.", Priority.React, true));

        AddEvent("Let's trade.", new ReactionTracker("The Trapper flips their mask and becomes The Trader, revealing that they are the same person.", Priority.React, true));

        AddEvent("<color=#D93846>Too fast. Too soon.</color>", new ReactionTracker(num => num switch
        {
            0 => ("The mysterious figure is a sore loser and is infuriated his boss encounters are getting defeated too fast. He resorts to cheating in order to prevent further progress! He plays an impassable wall of grizzlies.", Priority.React),
            1 => ("The mysterious figure resorts to cheating once again and plays another impassable wall of grizzlies...", Priority.React),
            >= 2 => ("Another wall of grizzlies is played by the cheating opponent.", Priority.Silent)
        }));

        AddEvent("You took your time out there.", new ReactionTracker("The mysterious figure finally appears visible in the light, he looks like an old man with leathery skin covered in leafage.", Priority.React, true));

        AddEvent("Now <color=#D93846>that</color> is value.", new ReactionTracker("The old man snapped a picture of the MOON ITSELF, capturing it within a card. This seems to be the last obstacle to defeat him.", Priority.React, true));

        AddEvent("<color=#D93846>Starvation</color> began to set in...", new ReactionTracker("Having no cards to draw, starvation appears on the battlefield and occupies a card space. It is a race against time as each turn starvation will spread until no card spaces are left, and the player loses.", Priority.React, true));

        AddEvent("Surely this food will please you.", new ReactionTracker("The reward for defeating the old man is a disgusting pile of meat with a lit candle on top.", Priority.React, true));

        AddEvent("The last challenger managed to defeat me.", new ReactionTracker("The mysterious figure refers to the last challenger winning against him. However, the cycle doesn't seem to be broken.", Priority.React, true));
    }

    private static readonly Dictionary<string, ReactionTracker> _events = [];

    private static void AddEvent(string dialogue, ReactionTracker reaction)
    {
        _events[dialogue.ToLowerInvariant()] = reaction;
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
