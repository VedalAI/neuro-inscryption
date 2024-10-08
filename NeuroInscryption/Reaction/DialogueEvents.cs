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
                SendContext("The survivors of the campfire offer shelter to only one creature, the chosen creature will gain more stat as a result. However, these survivors look famished and untrustworthy.", Priority.React);
                break;

            case "Heeeeeeee-haaw! 'Twas <color=#D93846>The Prospector</color>!":
                SendContext("The first boss encounter is the Prospector, his mule seems very dear to him.", Priority.Context);
                break;

            case "G-G-GOLD! I'VE STRUCK GOLD!":
                SendContext("The Prospector just struck all played cards replacing them with golden rocks! Those rocks will need to be cleared to play more creatures.", Priority.React);
                break;

            case "I am <color=#D93846>The Angler</color>. Go fish.":
                SendContext("The second boss encounter is the Angler, he will hook the last played card and use it has his own.", Priority.Context);
                break;

            case "Sorry. But today <color=#D93846>you</color> will supply the pelts.":
                SendContext("The third boss encounter is the Trapper, he will use traps to skin opposing creatures, thankfully the player gets to keep the pelts.", Priority.Context);
                break;

            case "<color=#D93846>Too fast. Too soon.</color>":
                SendContext("Leshy is infuriated his boss encounters are getting defeated too fast and places walls of grizzlies, a very difficult foe to vanquish.", Priority.React);
                break;

            case "You took your time out there.": // leshy first time visible
                SendContext("The mysterious figure known as Leshy finally appears visible in the light, he looks like an old man with leathery skin covered in leafage.", Priority.React);
                break;

            case "Now <color=#D93846>that</color> is value.": // moon
                SendContext("Leshy snapped a picture of the moon capturing it within a card, this seems to be the last obstacle to defeat Leshy.", Priority.React);
                break;

            case "We will work together to amend that.": // deathcard
                SendContext("Leshy request a new card to be created, elements of the player's cards will be used to piece it together.", Priority.Context);
                break;

            case "<color=#D93846>Starvation</color> began to set in...":
                SendContext("Having no card to draw, starvation appears on the battlefield and occupies a card space. It is a race against time as each turn starvation will spread until no card spaces are left.", Priority.React);
                break;

            case "Surely this food will please you.": // leshy cake
                SendContext("The reward for defeating Leshy is a pile of meat with a lit candle on top.", Priority.Context);
                break;

            case "The last challenger managed to defeat me.": // restart after winning
                SendContext("Leshy refers to the last challenger winning against him. However, the cycle doesn't seem to be broken.", Priority.React);
                break;


        }
    }

    // TODO: Certain events should only trigger once
    private static void SendContext(string context, Priority priority)
    {
        new Context(context, priority == Priority.Silent, priority == Priority.React).Send();
    }
}
