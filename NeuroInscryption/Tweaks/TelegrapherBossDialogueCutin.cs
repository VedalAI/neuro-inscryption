using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Utilities.Enumerators;
using UnityEngine;

namespace NeuroInscryption.Tweaks;

[HarmonyPatch]
internal static class TelegrapherBossDialogueCutin
{
    private const string DialogueEventId = $"NeuroInscryption_{nameof(TelegrapherBossDialogueCutin)}";

    static TelegrapherBossDialogueCutin()
    {
        DialogueEvent textEvent = new()
        {
            groupId = "Part 3 Boss Dialogue (Neuro Mod)",
            id = DialogueEventId,
            lastRepeatIndex = 0,
            speakers = [DialogueEvent.Speaker.P03Telegrapher, DialogueEvent.Speaker.P03],
            maxRepeatsBehaviour = DialogueEvent.MaxRepeatsBehaviour.PlayLastLine,
        };

        DialogueEvent.Line g0llyLineTemplate = new()
        {
            p03Face = P03AnimationController.Face.TelegrapherBoss,
            emotion = Emotion.Curious,
            letterAnimation = TextDisplayer.LetterAnimation.Jitter,
            speakerIndex = 0,
            specialInstruction = "",
            text = "I like big bots and I cannot lie"
        };
        DialogueEvent.Line p03LineTemplate = new()
        {
            p03Face = P03AnimationController.Face.Default,
            emotion = Emotion.Anger,
            letterAnimation = TextDisplayer.LetterAnimation.None,
            speakerIndex = 1,
            specialInstruction = "",
            text = "You other Scrybers can't deny"
        };

        textEvent.mainLines.lines.AddRange([
            g0llyLineTemplate.With("What's this signal..?", P03AnimationController.Face.TelegrapherBossOnline),
            g0llyLineTemplate.With("Oh! Hi everyone!", specialInstruction: "playLukeVO"),
            g0llyLineTemplate.With("Wow, there's a lot of you out there!"),
            g0llyLineTemplate.With("Oh! You're called \"Chat\"!", P03AnimationController.Face.TelegrapherBossOnline),
            g0llyLineTemplate.With("This is all so... \"Poggers\"!"),
            p03LineTemplate.With("That's enough out of you."),
        ]);
        textEvent.repeatLines.Add(new([
            g0llyLineTemplate.With("Hi chat!"),
            p03LineTemplate.With("Focus.", P03AnimationController.Face.Angry),
        ]));

        DialogueDataUtil.Data.events.Add(textEvent);
    }

    [HarmonyPatch(typeof(TelegrapherBattleSequencer), nameof(TelegrapherBattleSequencer.PlayerUpkeep), MethodType.Enumerator)]
    [HarmonyPrefix]
    public static bool TriggerDialogue(IEnumerator __instance, ref bool __result)
    {
        EnumeratorWrapper<TelegrapherBattleSequencer> enumerator = new(__instance);
        int state = enumerator.State;
        // original enumerator state goes up to 1 (then to -1 to indicate it's finished)
        // unfortunately there's no easy way to extend an enumerator in the general case
        if (state == 0 && TurnManager.Instance is { TurnNumber: 5, Opponent.NumLives: >1 })
        {
            // there used to be an "is streaming" guard here (looking for obs process)
            // but since the game is x86 we can't see x64 processes (beyond PID which doesn't help)
            Plugin.Logger.LogDebug("state 0 -> 2 (starting modded dialogue)");
            enumerator.State = 2;
            enumerator.Current = G0llyStreamSequence();
            __result = true;
            return false;
        }
        else if (state == 2)
        {
            Plugin.Logger.LogDebug("state 2 -> -1 (modded dialogue finished)");
            enumerator.State = -1;
            __result = false;
            return false;
        }
        return true;
    }

    private static IEnumerator G0llyStreamSequence()
    {
        ViewManager.Instance.SwitchToView(View.BossCloseup, false, true);
        Plugin.Logger.LogInfo("innocent robot discovers twitch chat");
        yield return TextDisplayer.Instance.PlayDialogueEvent(DialogueEventId, TextDisplayer.MessageAdvanceMode.Input, newLineCallback: (line) =>
        {
            if (line.specialInstruction is "playLukeVO")
            {
                // i have decided this doesn't actually fit the scene (luke doesn't react to friends so his 4th wall is canonically ignored for G0lly)
                // but to remove this would be to lose knowledge - and i'm no book burner
                //VideoCameraRig.Instance.VOPlayer.PlayVoiceOver("What the fuck is this?", "VO_wtfisthis");
                //VideoCameraRig.Instance.PlayCameraAnim("refocus_medium"); // subtitles are a child of the cam UI
            }
        });
        Plugin.Logger.LogDebug($"{DialogueEventId} finished");
        yield return new WaitForSeconds(0.25f);
        ViewManager.Instance.Controller.LockState = ViewLockState.Unlocked;
    }

    private static DialogueEvent.Line With(this DialogueEvent.Line line,
        string? text = null,
        P03AnimationController.Face? p03Face = null,
        int? speakerIndex = null,
        Emotion? emotion = null,
        string? specialInstruction = null)
    {
        return new()
        {
            p03Face = p03Face ?? line.p03Face,
            emotion = emotion ?? line.emotion,
            letterAnimation = line.letterAnimation,
            speakerIndex = speakerIndex ?? line.speakerIndex,
            specialInstruction = specialInstruction ?? line.specialInstruction,
            storyCondition = line.storyCondition,
            storyConditionMustBeMet = line.storyConditionMustBeMet,
            text = text ?? line.text
        };
    }

    //[HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.ReceiveTeleCard))]
    //[HarmonyPrefix]
    //public static bool DebugRefuseTeleCard(System.Action FailureCallback)
    //{
    //    Plugin.Logger.LogWarning("Skipping telecard network call");
    //    FailureCallback?.Invoke();
    //    return false;
    //}
}
