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

            
        }
    }

    private static void SendContext(string context, Priority priority)
    {
        new Context(context, priority == Priority.Silent, priority == Priority.React).Send();
    }
}