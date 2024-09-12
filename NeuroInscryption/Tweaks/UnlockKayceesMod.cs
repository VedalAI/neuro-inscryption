using DiskCardGame;
using HarmonyLib;

namespace NeuroInscryption.Tweaks;

[HarmonyPatch]
public static class UnlockKayceesMod
{
    [HarmonyPatch(typeof(SaveFile), nameof(SaveFile.IsAscension), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool SetKayceesActive(out bool __result)
    {
        if (SaveManager.SaveFile.ascensionData == null)
        {
            SaveManager.SaveFile.ascensionData = new AscensionSaveData();
            SaveManager.SaveFile.ascensionData.Initialize();
        }

        __result = true;
        return false;
    }

    [HarmonyPatch(typeof(StoryEventsData), nameof(StoryEventsData.EventCompleted))]
    [HarmonyPostfix]
    private static void EnableKayceesCard(ref bool __result, StoryEvent storyEvent)
    {
        if (storyEvent == StoryEvent.ChapterSelectUnlocked) __result = true;
    }
}
