using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NeuroInscryption.API.Input;
using NeuroInscryption.API.Utilities;
using UnityEngine;
using UnityEngine.Yoga;

namespace NeuroInscryption.API.Automation;

internal sealed class AutomationManager : Singlyboi<AutomationManager>
{
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

        // _currentFlow?.Interrupt();
        if (_currentFlowCoroutine != null) StopCoroutine(_currentFlowCoroutine);

        _currentFlow = flow;
        _currentFlowCoroutine = StartCoroutine(CoExecuteFlow(_currentFlow, instance));
    }

    // ReSharper disable once FunctionRecursiveOnAllPaths
    private static IEnumerator CoExecuteFlow(AutomationFlow flow, object instance)
    {
        IEnumerator executeEnumerator = flow.Execute(instance);

        yield return new WaitUntil(() => !NeuroMouse.AllowRegularInput);

        while (executeEnumerator.MoveNext())
        {
            yield return executeEnumerator.Current;

            if (NeuroMouse.AllowRegularInput)
            {
                yield return CoExecuteFlow(flow, instance); // Restart the flow after input is allowed again
                yield break;
            }
        }

        while (true)
        {
            yield return null;

            if (NeuroMouse.AllowRegularInput)
            {
                yield return CoExecuteFlow(flow, instance); // Restart the flow after input is allowed again
                yield break;
            }
        }
    }

    private readonly Dictionary<string, AutomationFlow> _flowCache = [];

    private static void Patch(object __instance, MethodInfo __originalMethod)
    {
        Instance.TryStartNewFlow(__originalMethod.FullDescription(), __instance);
    }
}
