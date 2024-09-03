using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.API.Automation;
using NeuroInscryption.API.Input;
using NeuroInscryption.Configuration;
using UnityEngine;
using UnityEngine.Yoga;
using Logger = HarmonyLib.Tools.Logger;

namespace NeuroInscryption.Automation;

internal sealed class ChallengesScreen : AutomationFlow<AscensionChallengeScreen>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionChallengeScreen), nameof(AscensionChallengeScreen.OnEnable));

    protected override IEnumerator Execute(AscensionChallengeScreen target)
    {
        yield return new WaitForSeconds(1);

        ChallengeFlags enabledChallenges = Plugin.Config.Challenges.Value;

        bool seenDifficulty = false;
        foreach (AscensionIconInteractable icon in target.icons.OrderBy(i => !i.transform.parent.name.Contains("Top")))
        {
            if (enabledChallenges.Includes(icon.Info.challengeType, seenDifficulty) != icon.activatedRenderer.enabled)
            {
                yield return SimulatedMouse.MoveTo(icon);
                yield return new WaitForSeconds(0.2f);
                SimulatedMouse.ClickOnce();
            }

            if (icon.Info.challengeType == AscensionChallenge.BaseDifficulty) seenDifficulty = true;
        }

        yield return SimulatedMouse.MoveTo(target.continueButton);
        yield return new WaitForSeconds(0.5f);
        SimulatedMouse.ClickOnce();
    }
}
