using System;
using System.Collections.Generic;
using System.Linq;
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
    private static readonly IReadOnlyList<IAutomationFlow> _allFlows = Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => !t.IsAbstract)
        .Where(t => typeof(IAutomationFlow).IsAssignableFrom(t))
        .Select(Activator.CreateInstance)
        .Cast<IAutomationFlow>()
        .ToList();

    private static readonly Dictionary<string, List<LinkedFlowHook>> _flowCache = [];
    public static FlowRunner FlowRunner { get; } = new();

    private static bool _allowManualInput;
    public static bool AllowManualInput
    {
        get => _allowManualInput;
        private set
        {
            if (_allowManualInput == value) return;
            if (value)
            {
                FlowRunner.CancelCurrentFlowForManualInput();
            }
            else
            {
                FlowRunner.RestartLastFlow();
            }
            _allowManualInput = value;
            InfoUI.SetManualInputActive(value);
        }
    }

    public static void TryStartFlow<T>() where T : ManualAutomationFlow
    {
        StartNewFlow(_allFlows.OfType<T>().First(), null);
    }

    private new void Awake()
    {
        base.Awake();

        foreach (AutomationFlow flow in _allFlows.OfType<AutomationFlow>())
        {
            foreach (FlowHook hook in flow.Hooks)
            {
                switch (hook.PatchType)
                {
                    case HarmonyPatchType.Prefix:
                        Plugin.Harmony.Patch(hook.Method, prefix: GetPatchMethod(hook));
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
        if (UnityEngine.Input.GetKeyDown(KeyCode.U) && FlowRunner.AllowManualInterruption)
        {
            AllowManualInput = !AllowManualInput;
            Plugin.Logger.LogWarning($"Manual input {(AllowManualInput ? "enabled" : "disabled")}");
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.P) && AllowManualInput)
        {
            FlowRunner.CancelCurrentFlowForcefully();
        }
    }

    private static void TryStartNewFlow(string description, HarmonyContext context)
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
                Plugin.Logger.LogInfo($"Skipping change to flow {linked.flow.GetType().Name} since current instance is not valid.");
                continue;
            }

            if (changed)
            {
                Plugin.Logger.LogError($"Multiple valid flows found for method description \"{description}\". {linked.flow.GetType().Name} will be ignored.");
                continue;
            }
            changed = true;

            StartNewFlow(linked.flow, context);
        }
    }

    private static void StartNewFlow(IAutomationFlow flow, HarmonyContext? context)
    {
        if (AllowManualInput)
        {
            FlowRunner.QueueFlow(flow, context);
        }
        else
        {
            FlowRunner.StartFlow(flow, context);
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
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, null, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPrefixStatic(MethodInfo __originalMethod, object __result, object[] __args)
    {
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, null, __result, __args));
    }

    [UsedImplicitly]
    private static void PatchPrefixVoid(MethodInfo __originalMethod, object __instance, object[] __args)
    {
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, __instance, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPrefix(MethodInfo __originalMethod, object __instance, object __result, object[] __args)
    {
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, __instance, __result, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfixStaticVoid(MethodInfo __originalMethod, object[] __args)
    {
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, null, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfixStatic(MethodInfo __originalMethod, object __result, object[] __args)
    {
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, null, __result, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfixVoid(MethodInfo __originalMethod, object __instance, object[] __args)
    {
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, __instance, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfix(MethodInfo __originalMethod, object __instance, object __result, object[] __args)
    {
        TryStartNewFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, __instance, __result, __args));
    }

    #endregion
}
