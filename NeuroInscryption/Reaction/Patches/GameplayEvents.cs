using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
public static class GameplayEvents
{
    [HarmonyPatch(typeof(Part1Opponent), nameof(Part1Opponent.DefeatedPlayerSequence), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void PlayerLost(object __instance)
    {
        if (__instance.GetEnumeratorState() != 1) return;

        if (RunState.Run.playerLives == 0) Reactions.PlayerLostRun.Send();
        else Reactions.PlayerLostBattle.Send(RunState.Run.playerLives switch
        {
            1 => "1 life",
            _ => $"{RunState.Run.playerLives} lives"
        });
    }
}