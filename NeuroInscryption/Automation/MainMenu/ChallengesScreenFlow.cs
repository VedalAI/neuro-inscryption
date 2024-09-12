using System.Collections;
using System.Linq;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Configuration;
using NeuroInscryption.Input;
using UnityEngine;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class ChallengesScreenFlow : AutomationFlowNoParam<AscensionChallengeScreen>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionChallengeScreen), nameof(AscensionChallengeScreen.OnEnable));

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute(AscensionChallengeScreen target)
    {
        yield return new WaitForSeconds(1);

        ChallengeFlags enabledChallenges = Plugin.Config.Challenges.Value;

        bool seenDifficulty = false;
        foreach (AscensionIconInteractable icon in target.icons.OrderBy(i => !i.transform.parent.name.Contains("Top")))
        {
            if (enabledChallenges.Includes(icon.Info.challengeType, seenDifficulty) != icon.activatedRenderer.enabled)
            {
                yield return NeuroMouse.MoveTo(icon);
                yield return new WaitForSeconds(0.2f);
                NeuroMouse.ClickOnce();
            }

            if (icon.Info.challengeType == AscensionChallenge.BaseDifficulty) seenDifficulty = true;
        }

        yield return NeuroMouse.MoveTo(target.continueButton);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
