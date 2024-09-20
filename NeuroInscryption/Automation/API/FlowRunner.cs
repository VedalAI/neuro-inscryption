using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NeuroInscryption.Automation.API;

public sealed class FlowRunner
{
    private AutomationFlow? _currentFlow;
    private HarmonyContext? _currentContext;
    private CancellationTokenSource? _cts;

    public string FlowName => _currentFlow?.GetType().Name ?? "<none>";

    public bool AllowManualInterruption => _currentFlow?.AllowManualInterruption ?? true;

    public void StartFlow(AutomationFlow flow, HarmonyContext context)
    {
        QueueFlow(flow, context);
        RestartLastFlow();
    }

    public void CancelCurrentFlowForManualInput()
    {
        _cts?.Cancel();

        if (_currentFlow?.ManualInterruptionBehaviour == ManualInterruptionBehaviour.Cancel)
        {
            Plugin.Logger.LogInfo($"Flow {FlowName} was cancelled for manual input and will not restart.");
            _currentFlow = null;
        }
    }

    public void QueueFlow(AutomationFlow flow, HarmonyContext context)
    {
        if (_currentFlow != null)
        {
            if (_currentFlow.FlowsBlockedForInterruption.Contains(flow.GetType()))
            {
                Plugin.Logger.LogMessage($"Preventing changing flow from {FlowName} to {flow.GetType().Name}.");
                return;
            }

            if (_currentFlow.GetType() == flow.GetType() && !_currentFlow.CanInterruptItself)
            {
                return;
            }
        }

        Plugin.Logger.LogMessage($"Changing flow from {FlowName} to {flow.GetType().Name}.");

        _cts?.Cancel();

        flow.ResetState();
        _currentFlow = flow;
        _currentContext = context;
    }

    public void RestartLastFlow()
    {
        if (_currentFlow == null) return;

        if (_cts?.IsCancellationRequested == false)
        {
            if (_currentFlow.FlowsBlockedForInterruption.Contains(_currentFlow.GetType()))
            {
                Plugin.Logger.LogInfo($"Restarting flow {FlowName} was blocked.");
                return;
            }

            if (!_currentFlow.CanInterruptItself)
            {
                return;
            }
        }

        Plugin.Logger.LogMessage($"Starting flow {FlowName}.");

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();
        ExecuteCurrentFlowAsync().Forget();
    }

    private async UniTaskVoid ExecuteCurrentFlowAsync()
    {
        try
        {
            object? instance = _currentContext!.GetInstance();
            if (instance == null || instance is UnityEngine.Object unityObject && unityObject == null)
            {
                Plugin.Logger.LogWarning($"Flow {FlowName} was cancelled because the target object was destroyed.");
                return;
            }

            if (instance is GameObject { activeInHierarchy: false })
            {
                Plugin.Logger.LogWarning($"Flow {FlowName} was cancelled because the target object was inactive.");
                return;
            }

            _currentFlow!.ResetState();
            await _currentFlow.ExecuteAsync(_currentContext, _cts!.Token);
            Plugin.Logger.LogMessage($"Flow {FlowName} completed.");
            _currentFlow = null;
        }
        catch (OperationCanceledException)
        {
            Plugin.Logger.LogMessage($"Flow {FlowName} was cancelled.");
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Caught exception while executing flow {FlowName}. Flow will be restarted");
            Plugin.Logger.LogError(e);
            RestartLastFlow();
        }
    }
}
