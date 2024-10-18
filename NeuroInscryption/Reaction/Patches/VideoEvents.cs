using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Reaction.API;
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

        VideoTracker? video = __instance.player.clip.name switch
        {
            "email_drafting" => Videos.Set2Video1,
            "question_to_audience" => Videos.Set2Video2,
            "email_response" => Videos.Set2Video3,
            "employee_visit" => Videos.Set2Video5,
            "not_normal" => Videos.Set2Video7,

            "garage_sale" => Videos.Set3Video1,
            "long_night_1" => Videos.Set3Video2,
            "long_night_2" => Videos.Set3Video3,
            "long_night_3" => Videos.Set3Video4,
            "break_in" => Videos.Set3Video6,
            "luke-insane" => Videos.Set3Video8,

            _ => null
        };

        video?.Send(__instance.player.time);
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
