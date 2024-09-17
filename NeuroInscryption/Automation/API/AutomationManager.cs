using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Tweaks;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Automation.API;

using LinkedFlowHook = (FlowHook hook, AutomationFlow flow);

[HarmonyPatch]
public sealed class AutomationManager : Singlyboi<AutomationManager>
{
    public const KeyCode ManualInputKey = KeyCode.U;

    private static readonly Dictionary<string, List<LinkedFlowHook>> _flowCache = [];
    private static readonly FlowRunner _flowRunner = new();

    private static bool _allowManualInput;
    public static bool AllowManualInput
    {
        get => _allowManualInput;
        private set
        {
            if (_allowManualInput == value) return;
            if (value)
            {
                _flowRunner.CancelCurrentFlow();
            }
            else
            {
                _flowRunner.RestartLastFlow();
            }
            _allowManualInput = value;
            ManualInputUI.SetActive(value);
        }
    }

    public static string CurrentFlowName => _flowRunner.FlowName;

    private new void Awake()
    {
        base.Awake();

        foreach (AutomationFlow flow in AutomationFlow.Flows)
        {
            foreach (FlowHook hook in flow.Hooks)
            {
                switch (hook.PatchType)
                {
                    case HarmonyPatchType.Prefix:
                        Plugin.Harmony.Patch(hook.Method, GetPatchMethod(hook));
                        break;
                    case HarmonyPatchType.Postfix:
                        Plugin.Harmony.Patch(hook.Method, postfix: GetPatchMethod(hook));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                string description = hook.GetDescription();
                if (!_flowCache.ContainsKey(description)) _flowCache[description] = [];
                _flowCache[description].Add((hook, flow));
            }
        }
    }

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(ManualInputKey))
        {
            AllowManualInput = !AllowManualInput;
            Plugin.Logger.LogWarning($"Manul input {(AllowManualInput ? "enabled" : "disabled")}");
        }
    }

    private void TryStartNewFlow(string description, HarmonyContext context)
    {
        if (!_flowCache.TryGetValue(description, out List<LinkedFlowHook> flows))
        {
            throw new ArgumentException($"No flows with method description \"{description}\" found.");
        }

        bool changed = false;
        foreach (LinkedFlowHook linked in flows)
        {
            if (!linked.hook.IsValid(context))
            {
                Plugin.Logger.LogDebug($"Skipping change to flow {linked.flow.GetType().Name} since current instance is not valid.");
                return;
            }

            if (changed)
            {
                Plugin.Logger.LogError($"Multiple valid flows found for method description \"{description}\". {linked.flow.GetType().Name} will be ignored.");
                continue;
            }
            changed = true;

            if (AllowManualInput)
            {
                _flowRunner.QueueFlow(linked.flow, context);
            }
            else
            {
                _flowRunner.StartFlow(linked.flow, context);
            }
        }
    }

    #region Patch methods

    private static HarmonyMethod GetPatchMethod(FlowHook hook)
    {
        string name = $"Patch{hook.PatchType}";
        if (hook.Method.IsStatic) name += "Static";
        if (hook.Method.ReturnType == typeof(void)) name += "Void";

        return new HarmonyMethod(AccessTools.Method(typeof(AutomationManager), name));
    }

    [UsedImplicitly]
    private static void PatchPrefixStaticVoid(MethodInfo __originalMethod, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, null, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPrefixStatic(MethodInfo __originalMethod, object __result, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, null, __result, __args));
    }

    [UsedImplicitly]
    private static void PatchPrefixVoid(MethodInfo __originalMethod, object __instance, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, __instance, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPrefix(MethodInfo __originalMethod, object __instance, object __result, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, __instance, __result, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfixStaticVoid(MethodInfo __originalMethod, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, null, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfixStatic(MethodInfo __originalMethod, object __result, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, null, __result, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfixVoid(MethodInfo __originalMethod, object __instance, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, __instance, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfix(MethodInfo __originalMethod, object __instance, object __result, object[] __args)
    {
        Instance.TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, __instance, __result, __args));
    }

    #endregion
}
