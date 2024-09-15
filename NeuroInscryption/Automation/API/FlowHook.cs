using System;
using System.Reflection;
using HarmonyLib;

namespace NeuroInscryption.Automation.API;

public sealed class FlowHook
{
    public required MethodBase Method { get; init; }
    public HarmonyPatchType PatchType { get; init; } = HarmonyPatchType.Postfix;
    public Func<HarmonyContext, bool> IsValid { get; init; } = _ => true;

    public string GetDescription() => $"{Method.FullDescription()}_{PatchType}";
}
