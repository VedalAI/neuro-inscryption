using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Reaction.Strings;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class VideoEvents
{
    [HarmonyPatch(typeof(FootageController), nameof(FootageController.Pause))]
    [HarmonyPatch(typeof(FootageController), nameof(FootageController.OnStopPressed))]
    [HarmonyPrefix]
    private static void HandleContext(FootageController __instance)
    {
        if (__instance.subsData == null) return;

        switch (__instance.subsData.name)
        {
            case "Subs_Blooper":
                Videos.Video1.Send(__instance.player.time);
                break;

            case "Subs_The-Lucky-Carder":
                Videos.Video2.Send(__instance.player.time);
                break;

            case "Subs_Strange-Inscription":
                Videos.Video3.Send(__instance.player.time);
                break;

            case "Subs_Forest-1":
                Videos.Video4.Send(__instance.player.time);
                break;

            case "Subs_Forest-2":
                Videos.Video6.Send(__instance.player.time);
                break;

            case "Subs_Buy-Floppy-Drive":
                Videos.Video7.Send(__instance.player.time);
                break;

            case "Subs_Inserting-Disk":
                Videos.Video8.Send(__instance.player.time);
                break;
        }

        new RequestMute(false).Send();
    }

    [HarmonyPatch(typeof(FootageController), nameof(FootageController.Play))]
    [HarmonyPostfix]
    private static void OnVideoStart(FootageController __instance)
    {
        if (__instance.subsData == null) return;

        new RequestMute(true).Send();
    }
}