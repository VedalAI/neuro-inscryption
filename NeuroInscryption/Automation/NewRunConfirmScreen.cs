using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.API.Automation;
using NeuroInscryption.API.Input;
using UnityEngine;

namespace NeuroInscryption.Automation;

internal sealed class NewRunConfirmScreen : AutomationFlow<AscensionMenuScreenTransition>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(AscensionMenuScreenTransition), nameof(AscensionMenuScreenTransition.OnEnable));

    protected override bool IsValid(AscensionMenuScreenTransition target)
    {
        return target.name == "AscensionNewRunConfirmScreen";
    }

    protected override IEnumerator Execute(AscensionMenuScreenTransition target)
    {
        yield return NeuroMouse.MoveTo(target.screenInteractables[1]);
        yield return new WaitForSeconds(0.5f);
        NeuroMouse.ClickOnce();
    }
}
