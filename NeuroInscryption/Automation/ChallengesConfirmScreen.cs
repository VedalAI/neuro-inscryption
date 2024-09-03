using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.API.Automation;
using NeuroInscryption.API.Input;
using UnityEngine;

namespace NeuroInscryption.Automation;

internal sealed class ChallengesConfirmScreen : AutomationFlow<AscensionChallengeConfirmScreen>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionChallengeConfirmScreen), nameof(AscensionChallengeConfirmScreen.OnEnable));

    protected override IEnumerator Execute(AscensionChallengeConfirmScreen target)
    {
        yield return NeuroMouse.MoveTo(target.continueButton);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
