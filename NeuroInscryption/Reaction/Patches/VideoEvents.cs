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
        switch (__instance.subsData != null ? __instance.subsData.name : null)
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

            case null:
                Videos.Video5.Send(__instance.player.time);
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
    }

    [HarmonyPatch(typeof(FootageController), nameof(FootageController.Play))]
    [HarmonyPrefix]
    private static void OnVideoStart()
    {
        new RequestMute(true).Send();
    }

    [HarmonyPatch(typeof(FootageController), nameof(FootageController.Pause))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    private static void OnVideoStop()
    {
        new RequestMute(false).Send();
    }
}