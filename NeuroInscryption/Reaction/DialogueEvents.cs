using System;
using System.Collections.Generic;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Messages.Outgoing;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
public static class DialogueEvents
{
    static DialogueEvents()
    {
        // Note: Most of these dialogue lines only happen on the first occurrence of the event.

        AddEvent("You noticed one of the survivors wiping drool from their mouth.", num => num switch
        {
            0 => ("The survivors of the campfire offer shelter to a creature. They claim that the chosen creature will gain a buff as a result, however, these survivors look famished and untrustworthy.", Priority.React),
            _ => null
        });

        AddEvent("Heeeeeeee-haaw! 'Twas <color=#D93846>The Prospector</color>!", num => num switch
        {
            0 => ("A hobbled figure appeared... the first boss encounter: 'Heeeeeeee-haaw! 'Twas The Prospector!'. His mule seems very dear to him.", Priority.React),
            _ => null
        });

        AddEvent("G-G-GOLD! I'VE STRUCK GOLD!", num => num switch
        {
            0 => ("The Prospector just struck all played cards, killing them and replacing them with useless golden nuggets! He is breaking the rules of the game!", Priority.React),
            1 => ("The Prospector replaced all cards with gold nuggets again.", Priority.Silent),
            _ => null
        });

        AddEvent("I am <color=#D93846>The Angler</color>. Go fish.", num => num switch
        {
            0 => ("A hulking man sat by a brackish pond with his feet submerged in the dark water. He pulled a hook from a pile of rotting fish and rose to his feet. The second boss encounter is the Angler. Go fish.", Priority.React),
            _ => null
        });

        AddEvent("Sorry. But today <color=#D93846>you</color> will supply the pelts.", num =>  num switch
        {
            0 => ("You took in the familiar sights and scents of The Trapper's pelts. However, the once friendly man now beheld you with a steelzy gaze. The third boss encounter is starting.", Priority.React),
            _ => null
        });

        AddEvent("<color=#D93846>Too fast. Too soon.</color>", num => num switch
        {
            0 => ("The mysterious figure is a sore loser and is infuriated his boss encounters are getting defeated too fast. He resorts to cheating in order to prevent further progress! He plays an impassable wall of grizzlies.", Priority.React),
            1 => ("The mysterious figure resorts to cheating once again and plays another impassable wall of grizzlies...", Priority.React),
            >= 2 => ("Another wall of grizzlies is played by the cheating opponent.", Priority.Silent)
        });

        AddEvent("You took your time out there.", num => num switch
        {
            0 => ("The mysterious figure finally appears visible in the light, he looks like an old man with leathery skin covered in leafage.", Priority.React),
            _ => null
        });

        AddEvent("Now <color=#D93846>that</color> is value.", num => num switch
        {
            0 => ("The old man snapped a picture of the MOON ITSELF, capturing it within a card. This seems to be the last obstacle to defeat him.", Priority.React),
            _ => null
        });

        AddEvent("<color=#D93846>Starvation</color> began to set in...", num => num switch
        {
            0 => ("Having no cards to draw, starvation appears on the battlefield and occupies a card space. It is a race against time as each turn starvation will spread until no card spaces are left, and the player loses.", Priority.React),
            _ => null
        });

        AddEvent("Surely this food will please you.", num => num switch
        {
            0 => ("The reward for defeating the old man is a disgusting pile of meat with a lit candle on top.", Priority.Context),
            _ => null
        });

        AddEvent("The last challenger managed to defeat me.", num => num switch
        {
            0 => ("The mysterious figure refers to the last challenger winning against him. However, the cycle doesn't seem to be broken.", Priority.React),
            _ => null
        });
    }

    private enum Priority
    {
        React,   // Neuro will react to the prompt
        Context, // Neuro may or may not react to the prompt, her choice
        Silent   // Neuro will not react to the prompt, but it will still be added to her context window
    }

    private record struct Event(string Context, Priority Priority)
    {
        public static implicit operator Event((string context, Priority priority) tuple) => new(tuple.context, tuple.priority);
    }

    private static readonly Dictionary<string, Func<uint, Event?>> _events = [];
    private static readonly Dictionary<string, uint> _occurrences = [];

    private static void AddEvent(string dialogue, Func<uint, Event?> getterFunc)
    {
        _events[dialogue] = getterFunc;
    }

    [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowMessage))]
    [HarmonyPrefix]
    private static void HandleLine(string message)
    {
        Plugin.Logger.LogWarning(message);

        if (_events.TryGetValue(message.Trim(), out Func<uint, Event?>? getterFunc))
        {
            Event? result = getterFunc(_occurrences.TryGetValue(message.Trim(), out uint count) ? count : 0);
            if (result != null) SendContext(result.Value);
            _occurrences[message.Trim()] = count + 1;
        }
    }

    private static void SendContext(Event @event)
    {
        new Context(@event.Context, @event.Priority == Priority.Silent, @event.Priority == Priority.React).Send();
    }
}
