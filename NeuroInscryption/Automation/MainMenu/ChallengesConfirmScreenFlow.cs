using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using UnityEngine;

namespace NeuroInscryption.Automation.MainMenu;

public sealed class ChallengesConfirmScreenFlow : AutomationFlow<AscensionChallengeConfirmScreen>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(AscensionChallengeConfirmScreen), nameof(AscensionChallengeConfirmScreen.OnEnable))
        }
    ];

    protected override IEnumerator Execute(AscensionChallengeConfirmScreen target)
    {
        yield return NeuroMouse.MoveTo(target.continueButton);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
