using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using NeuroInscryption.Tweaks;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Automation.API;

using LinkedFlowHook = (FlowHook hook, HarmonyAutomationFlow flow);

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

    public static void StartFlowManually<TFlow, TContext>(TContext context) where TFlow : AutomationFlow<TContext> where TContext : notnull
    {
        StartFlow(_allFlows.OfType<TFlow>().First(), context);
    }

    private new void Awake()
    {
        base.Awake();

        foreach (HarmonyAutomationFlow flow in _allFlows.OfType<HarmonyAutomationFlow>())
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
        if (UnityEngine.Input.GetKeyDown(KeyCode.U))
        {
            bool allowToggle = AllowManualInput || FlowRunner.AllowManualInterruption;
#if DEBUG
            allowToggle |= UnityEngine.Input.GetKey(KeyCode.LeftControl) && UnityEngine.Input.GetKey(KeyCode.LeftShift);
#endif

            if (allowToggle)
            {
                AllowManualInput = !AllowManualInput;
                Plugin.Logger.LogWarning($"Manual input {(AllowManualInput ? "enabled" : "disabled")}");
            }
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.P) && AllowManualInput)
        {
            FlowRunner.CancelCurrentFlowForcefully();
        }
    }

    private static void TryStartHarmonyFlow(string description, HarmonyContext context)
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
                // Plugin.Logger.LogDebug($"Skipping change to flow {linked.flow.GetType().Name} since current instance is not valid.");
                continue;
            }

            if (changed)
            {
                Plugin.Logger.LogError($"Multiple valid flows found for method description \"{description}\". {linked.flow.GetType().Name} will be ignored.");
                continue;
            }
            changed = true;

            StartFlow(linked.flow, context);
        }
    }

    private static void StartFlow(IAutomationFlow flow, object context)
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

    [Conditional("DEBUG")]
    public static void Assert([DoesNotReturnIf(false)] bool condition, string? message = null,
        string? details = null,
        [CallerFilePath] string? callerFile = null,
        [CallerMemberName] string? callerMember = null,
        [CallerLineNumber] int? line = null)
    {
        if (condition) return;

        Plugin.Logger.LogWarning("Forcing manual input due to assertion failure");
        AllowManualInput = true;
        FlowRunner.CancelCurrentFlowForcefully();

        StringBuilder sb = new("Assertion failed: ");
        sb.AppendLine(message ?? "(no message)");

        int projNameIdx = Math.Max(0, callerFile!.IndexOf(nameof(NeuroInscryption), StringComparison.Ordinal));
        sb.AppendLine($"\tin {callerFile[projNameIdx ..]}");
        sb.AppendLine($"\tat {callerMember}:{line}");
        if (!string.IsNullOrEmpty(details))
        {
            sb.Append("Details: ");
            sb.AppendLine(details);
        }
        sb.AppendLine("Stack trace:");
        throw new Exception(sb.ToString());
    }

    #region Patch methods

    private static HarmonyMethod GetPatchMethod(FlowHook hook)
    {
        Delegate method = (hook.PatchType, hook.Method.ReturnType == typeof(void)) switch
        {
            (HarmonyPatchType.Prefix, false) => PatchPrefix,
            (HarmonyPatchType.Postfix, false) => PatchPostfix,
            (HarmonyPatchType.Prefix, true) => PatchPrefixVoid,
            (HarmonyPatchType.Postfix, true) => PatchPostfixVoid,
            _ => throw new InvalidOperationException($"Invalid hook patch type {hook.PatchType}")
        };

        return new HarmonyMethod(method.Method);
    }

    [UsedImplicitly]
    private static void PatchPrefixVoid(MethodInfo __originalMethod, object? __instance, object[] __args)
    {
        TryStartHarmonyFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, __instance, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPrefix(MethodInfo __originalMethod, object? __instance, object? __result, object[] __args)
    {
        TryStartHarmonyFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Prefix), new HarmonyContext(__originalMethod, __instance, __result, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfixVoid(MethodInfo __originalMethod, object? __instance, object[] __args)
    {
        TryStartHarmonyFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, __instance, null, __args));
    }

    [UsedImplicitly]
    private static void PatchPostfix(MethodInfo __originalMethod, object? __instance, object? __result, object[] __args)
    {
        TryStartHarmonyFlow(FlowHook.GetDescription(__originalMethod, HarmonyPatchType.Postfix), new HarmonyContext(__originalMethod, __instance, __result, __args));
    }

    #endregion
}
