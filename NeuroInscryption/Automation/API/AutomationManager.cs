using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace NeuroInscryption.Automation.API;

[HarmonyPatch]
public sealed class AutomationManager : Singlyboi<AutomationManager>
{
    public bool AllowManualInput { get; private set; }
    private bool _wantsManualControl;

    private AutomationFlow _currentFlow;
    private Coroutine _currentFlowCoroutine;

    private new void Awake()
    {
        base.Awake();

        foreach (AutomationFlow flow in AutomationFlow.Flows)
        {
            switch (flow.PatchType)
            {
                case HarmonyPatchType.Prefix:
                    Plugin.Harmony.Patch(flow.EnableHook, new HarmonyMethod(typeof(AutomationManager), nameof(Patch)));
                    break;
                case HarmonyPatchType.Postfix:
                    Plugin.Harmony.Patch(flow.EnableHook, postfix: new HarmonyMethod(typeof(AutomationManager), nameof(Patch)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _flowCache[flow.EnableHook.FullDescription()] = flow;
        }
    }

    private void TryStartNewFlow(string id, object instance)
    {
        if (!_flowCache.TryGetValue(id, out AutomationFlow flow))
        {
            throw new ArgumentException($"No flow with method ID \"{id}\" found.");
        }

        if (!flow.IsValid(instance))
        {
            Plugin.Logger.LogDebug($"Skipping change to flow {flow.GetType().Name} since current instance is not valid.");
            return;
        }

        Plugin.Logger.LogInfo($"Changing flow from {_currentFlow?.GetType().Name} to {flow.GetType().Name}.");

        if (_currentFlowCoroutine != null)
        {
            StopCoroutine(_currentFlowCoroutine);
            TryGiveManualControl();
        }

        _currentFlow = flow;
        _currentFlowCoroutine = StartCoroutine(CoExecuteFlow(_currentFlow, instance));
    }

    // ReSharper disable once FunctionRecursiveOnAllPaths
    private IEnumerator CoExecuteFlow(AutomationFlow flow, object instance)
    {
        IEnumerator executeEnumerator = flow.Execute(instance);

        yield return new WaitUntil(() => !AllowManualInput);

        while (executeEnumerator.MoveNext())
        {
            yield return executeEnumerator.Current;

            if (AllowManualInput)
            {
                yield return CoExecuteFlow(flow, instance); // Restart the flow after input is allowed again
                yield break;
            }
        }

        while (true)
        {
            yield return null;

            TryGiveManualControl();

            if (AllowManualInput)
            {
                yield return CoExecuteFlow(flow, instance); // Restart the flow after input is allowed again
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

    private readonly Dictionary<string, AutomationFlow> _flowCache = [];

    private static void Patch(object __instance, MethodInfo __originalMethod)
    {
        Instance.TryStartNewFlow(__originalMethod.FullDescription(), __instance);
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
            if (Instance._currentFlow.AllowInterruption)
            {
                Instance.AllowManualInput = true;
            }
            else
            {
                Instance._wantsManualControl = true;
            }
        }
    }
}
