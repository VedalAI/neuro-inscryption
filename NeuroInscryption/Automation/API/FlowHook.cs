using System;
using System.Reflection;
using HarmonyLib;

namespace NeuroInscryption.Automation.API;

public sealed class FlowHook
{
    public required MethodInfo Method { get; init; }
    public HarmonyPatchType PatchType { get; init; } = HarmonyPatchType.Postfix;
    public Func<HarmonyContext, bool> IsValid { get; init; } = _ => true;

    public string GetDescription() => GetDescription(Method, PatchType);

    public static string GetDescription(MethodInfo method, HarmonyPatchType patchType) => $"{method.FullDescription()}_{patchType}";
}
