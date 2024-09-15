using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Automation.API;

using LinkedFlowHook = (FlowHook hook, AutomationFlow flow);

[HarmonyPatch]
public sealed class AutomationManager : Singlyboi<AutomationManager>
{
    private readonly Dictionary<string, List<LinkedFlowHook>> _flowCache = [];

    public bool AllowManualInput { get; private set; }
    private bool _wantsManualControl;

    private AutomationFlow? _currentFlow;
    private Coroutine? _currentFlowCoroutine;

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

            Plugin.Logger.LogInfo($"Changing flow from {_currentFlow?.GetType().Name} to {linked.flow.GetType().Name}.");

            if (_currentFlowCoroutine != null)
            {
                StopCoroutine(_currentFlowCoroutine);
                TryGiveManualControl();
            }

            _currentFlow = linked.flow;
            _currentFlowCoroutine = StartCoroutine(CoExecuteFlow(_currentFlow, context));
        }
    }

    // ReSharper disable once FunctionRecursiveOnAllPaths
    private IEnumerator CoExecuteFlow(AutomationFlow flow, HarmonyContext context)
    {
        IEnumerator executeEnumerator = flow.Execute(context);

        yield return new WaitUntil(() => !AllowManualInput);

        while (executeEnumerator.MoveNext())
        {
            yield return executeEnumerator.Current;

            if (AllowManualInput)
            {
                yield return CoExecuteFlow(flow, context); // Restart the flow after input is allowed again
                yield break;
            }
        }

        while (true)
        {
            yield return null;

            TryGiveManualControl();

            if (AllowManualInput)
            {
                yield return CoExecuteFlow(flow, context); // Restart the flow after input is allowed again
                yield break;
            }
        }
    }

    private void TryGiveManualControl()
    {
        if (_wantsManualControl)
        {
            AllowManualInput = true;
            _wantsManualControl = false;
        }
    }

    [HarmonyPatch(typeof(InteractionCursor), nameof(InteractionCursor.UpdatePosition))]
    [HarmonyPrefix]
    private static void ManualControlHandler()
    {
        if (!UnityEngine.Input.GetKeyDown(KeyCode.U)) return;

        if (Instance.AllowManualInput)
        {
            Instance.AllowManualInput = false;
        }
        else
        {
            if (Instance._currentFlow?.AllowInterruption ?? true)
            {
                Instance.AllowManualInput = true;
            }
            else
            {
                Instance._wantsManualControl = true;
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
