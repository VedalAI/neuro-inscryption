using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Strings;
using NeuroSdk.Messages.Outgoing;

namespace NeuroInscryption.Communication.ContextMessages;

[HarmonyPatch]
file static class Rules
{
    [HarmonyPatch(typeof(TurnManager), nameof(TurnManager.StartGame), typeof(EncounterData))]
    [HarmonyPostfix]
    private static void Postfix()
    {
        Context.Send(ContextStrings.Rules, true);
    }
}