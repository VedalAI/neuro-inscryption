using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Messages.Outgoing;

namespace NeuroInscryption.Reaction;

[HarmonyPatch]
public static class DialogueEvents
{
    private enum Priority
    {
        React,   // Neuro will react to the prompt
        Context, // Neuro may or may not react to the prompt, her choice
        Silent   // Neuro will not react to the prompt, but it will still be added to her context window
    }

    [HarmonyPatch(typeof(TextDisplayer), nameof(TextDisplayer.ShowMessage))]
    [HarmonyPrefix]
    private static void HandleLine(string message)
    {
        Plugin.Logger.LogWarning(message);

        switch (message.Trim())
        {
            case "You noticed one of the survivors wiping drool from their mouth.":
                SendContext("", Priority.React);
                break;

            case "Heeeeeeee-haaw! 'Twas <color=#D93846>The Prospector</color>!":
                SendContext("", Priority.Context);
                break;

            case "G-G-GOLD! I'VE STRUCK GOLD!":
                SendContext("", Priority.React);
                break;

            case "I am <color=#D93846>The Angler</color>. Go fish.":
                SendContext("", Priority.Context);
                break;

            case "Sorry. But today <color=#D93846>you</color> will supply the pelts.":
                SendContext("", Priority.Context);
                break;

            case "<color=#D93846>Too fast. Too soon.</color>":
                SendContext("", Priority.React);
                break;

            case "You took your time out there.": // leshy first time visible
                SendContext("", Priority.React);
                break;

            case "Now <color=#D93846>that</color> is value.": // moon
                SendContext("", Priority.React);
                break;

            case "We will work together to amend that.": // deathcard
                SendContext("", Priority.Context);
                break;

            case "<color=#D93846>Starvation</color> began to set in...":
                SendContext("", Priority.React);
                break;

            case "Surely this food will please you.": // leshy cake
                SendContext("", Priority.Context);
                break;

            case "The last challenger managed to defeat me.": // restart after winning
                SendContext("", Priority.React);
                break;


        }
    }

    // TODO: Certain events should only trigger once
    private static void SendContext(string context, Priority priority)
    {
        new Context(context, priority == Priority.Silent, priority == Priority.React).Send();
    }
}