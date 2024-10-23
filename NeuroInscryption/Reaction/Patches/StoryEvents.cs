using System.Collections;
using System.IO;
using DiskCardGame;
using GBC;
using HarmonyLib;
using NeuroInscryption.Reaction.Strings;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class StoryEvents
{
    [HarmonyPatch(typeof(VideoCameraRig), nameof(VideoCameraRig.EnterFootageMenu))]
    [HarmonyPostfix]
    private static void EnterFootageMenu()
    {
        Reactions.VideoBrowsingStarted.Send();
    }

    #region Act 2

    [HarmonyPatch(typeof(DredgingMachine), nameof(DredgingMachine.Drop), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void DredgerDigsUpOldData(IEnumerator __instance)
    {
        if (__instance.Wrap<DredgingMachine>() is { State: 3, This.specialBotQueued: true })
        {
            Reactions.OldDataDugUp.Send();
        }
    }

    [HarmonyPatch(typeof(StartingIslandFinaleSequencer), nameof(StartingIslandFinaleSequencer.PostBattleFinaleSequence), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void Skynet(IEnumerator __instance)
    {
        if (__instance.GetState() == 0)
        {
            Reactions.Act2Ending.Send();
        }
    }

    #endregion Act 2

    #region Act 3

    [HarmonyPatch(typeof(Opponent), nameof(Opponent.QueueCard), MethodType.Enumerator)]
    [HarmonyPostfix]
    private static void OnCardPlayed(IEnumerator __instance)
    {
        EnumeratorWrapper<Opponent> enumerator = new(__instance);
        if (enumerator.State != 1) return;

        CardInfo cardInfo = enumerator.GetFieldValue<CardInfo>("cardInfo");
        if (cardInfo.name != "!FRIENDCARD_BASE") return;

        Reactions.G0llyFriendSpawned.Send(cardInfo.DisplayedNameEnglish);
    }

    [HarmonyPatch(typeof(ArchivistBossOpponent), nameof(ArchivistBossOpponent.FindFileSequence), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void FilePicked(IEnumerator __instance)
    {
        if (__instance.GetState() == 4)
        {
            FileInfo file = __instance.GetFieldValue<object>("<>8__1").GetFieldValue<FileInfo>("fileInfo");
            Reactions.ArchivistPickFile.Send(file.Name);
        }
    }

    [HarmonyPatch(typeof(AudioController), nameof(AudioController.PlaySound3D))]
    [HarmonyPostfix]
    private static void AightImmaHeadOut(string soundId)
    {
        if (soundId == "p03_head_off")
        {
            CustomCoroutine.WaitThenExecute(3.5f, () => Reactions.GoodbyeP03.Send());
        }
    }

    #endregion Act 3

    #region Finale

    [HarmonyPatch(typeof(GrimoraAnimationController), nameof(GrimoraAnimationController.WaitForHandshake), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void GrimoraGoodbye(IEnumerator __instance)
    {
        if (__instance.GetState() == 0)
        {
            Reactions.GoodbyeGrimora.Send();
        }
    }

    [HarmonyPatch(typeof(OpponentArmController), nameof(OpponentArmController.WaitForHandshake), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void LeshyGoodbye(IEnumerator __instance)
    {
        if (__instance.GetState() == 0)
        {
            Reactions.GoodbyeLeshy.Send();
        }
    }

    [HarmonyPatch(typeof(MagnificusGameFlowManager), nameof(MagnificusGameFlowManager.EndBattleAndTransition), MethodType.Enumerator)]
    [HarmonyPrefix]
    private static void MagnificusGoodbye(IEnumerator __instance)
    {
        if (__instance.GetState() == 12)
        {
            Reactions.GoodbyeMagnificus.Send();
        }
    }
    #endregion Finale
}