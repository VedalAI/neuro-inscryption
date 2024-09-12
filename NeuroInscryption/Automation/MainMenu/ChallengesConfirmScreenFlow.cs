using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using UnityEngine;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class ChallengesConfirmScreenFlow : AutomationFlowNoParam<AscensionChallengeConfirmScreen>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionChallengeConfirmScreen), nameof(AscensionChallengeConfirmScreen.OnEnable));

    public override bool AllowInterruption => true;

    protected override IEnumerator Execute(AscensionChallengeConfirmScreen target)
    {
        yield return NeuroMouse.MoveTo(target.continueButton);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
