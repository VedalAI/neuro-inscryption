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
            id = DialogueEventId,
            lastRepeatIndex = 0,
            speakers = [DialogueEvent.Speaker.P03Telegrapher, DialogueEvent.Speaker.P03],
            maxRepeatsBehaviour = DialogueEvent.MaxRepeatsBehaviour.PlayLastLine,
        };

        DialogueEvent.Line gollyLineTemplate = new()
        {
            p03Face = P03AnimationController.Face.TelegrapherBoss,
            emotion = Emotion.Curious,
            letterAnimation = TextDisplayer.LetterAnimation.Jitter,
            speakerIndex = 0,
            specialInstruction = "",
        };

        DialogueEvent.Line p03LineTemplate = new()
        {
            p03Face = P03AnimationController.Face.Default,
            emotion = Emotion.Anger,
            letterAnimation = TextDisplayer.LetterAnimation.None,
            speakerIndex = 1,
            specialInstruction = "",
        };

        textEvent.mainLines.lines.AddRange([
            gollyLineTemplate.With("What's this signal..?", P03AnimationController.Face.TelegrapherBossOnline),
            gollyLineTemplate.With("Oh! Hi everyone!", specialInstruction: "playLukeVO"),
            gollyLineTemplate.With("Wow, there's a lot of you out there!"),
            gollyLineTemplate.With("Oh! You're called \"Chat\"!", P03AnimationController.Face.TelegrapherBossOnline),
            gollyLineTemplate.With("This is all so... \"Poggers\"!"),
            p03LineTemplate.With("That's enough out of you."),
        ]);
        textEvent.repeatLines.Add(new DialogueEvent.LineSet([
            gollyLineTemplate.With("Hi chat!"),
            p03LineTemplate.With("Focus.", P03AnimationController.Face.Angry),
        ]));

        DialogueDataUtil.Data.events.Add(textEvent);
    }

    [HarmonyPatch(typeof(TelegrapherBattleSequencer), nameof(TelegrapherBattleSequencer.PlayerUpkeep))]
    [HarmonyPostfix]
    public static void TriggerDialogue(ref IEnumerator __result)
    {
        __result = gollyStreamSequence(__result);
        return;

        static IEnumerator gollyStreamSequence(IEnumerator original)
        {
            if (TurnManager.Instance is not { TurnNumber: 4, Opponent.NumLives: > 1 })
            {
                yield return original;
                yield break;
            }

            Plugin.Logger.LogInfo("innocent robot discovers twitch chat");

            ViewManager.Instance.SwitchToView(View.BossCloseup, false, true);
            yield return TextDisplayer.Instance.PlayDialogueEvent(DialogueEventId, TextDisplayer.MessageAdvanceMode.Input);
            yield return new WaitForSeconds(0.25f);
            ViewManager.Instance.Controller.LockState = ViewLockState.Unlocked;
        }
    }

    private static DialogueEvent.Line With(this DialogueEvent.Line line,
        string text,
        P03AnimationController.Face? p03Face = null,
        int? speakerIndex = null,
        Emotion? emotion = null,
        string? specialInstruction = null)
    {
        return new DialogueEvent.Line
        {
            p03Face = p03Face ?? line.p03Face,
            emotion = emotion ?? line.emotion,
            letterAnimation = line.letterAnimation,
            speakerIndex = speakerIndex ?? line.speakerIndex,
            specialInstruction = specialInstruction ?? line.specialInstruction,
            storyCondition = line.storyCondition,
            storyConditionMustBeMet = line.storyConditionMustBeMet,
            text = text,
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