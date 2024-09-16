using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace NeuroInscryption.Automation.API;

public sealed class FlowRunner
{
    private AutomationFlow? _currentFlow;
    private HarmonyContext? _currentContext;
    private CancellationTokenSource? _cts;

    private string FlowName => _currentFlow?.GetType().Name ?? "<none>";

    public void StartFlow(AutomationFlow flow, HarmonyContext context)
    {
        QueueFlow(flow, context);
        RestartLastFlow();
    }

    public void CancelCurrentFlow()
    {
        _cts?.Cancel();
    }

    public void QueueFlow(AutomationFlow flow, HarmonyContext context)
    {
        if (_currentFlow != null && _currentFlow.FlowsBlockedForInterruption.Contains(flow.GetType()))
        {
            if (_currentFlow.GetType() == flow.GetType())
            {
                Plugin.Logger.LogInfo($"Prevented changing flow from {FlowName} to itself.");
            }
            else
            {
                Plugin.Logger.LogMessage($"Preventing changing flow from {FlowName} to {flow.GetType().Name}.");
            }

            return;
        }

        Plugin.Logger.LogMessage($"Changing flow from {FlowName} to {flow.GetType().Name}.");

        _cts?.Cancel();

        _currentFlow = flow;
        _currentContext = context;
    }

    public void RestartLastFlow()
    {
        if (_currentFlow == null) return;

        if (_cts?.IsCancellationRequested == false && _currentFlow.FlowsBlockedForInterruption.Contains(_currentFlow.GetType()))
        {
            Plugin.Logger.LogInfo($"Restarting flow {FlowName} was blocked.");
            return;
        }

        Plugin.Logger.LogMessage($"Starting flow {FlowName}.");

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();
        ExecuteFlowAsync(_currentFlow, _currentContext!, _cts.Token).Forget();
    }

    private async UniTaskVoid ExecuteFlowAsync(AutomationFlow flow, HarmonyContext context, CancellationToken ct)
    {
        try
        {
            object? instance = context.GetInstance();
            if (instance == null || instance is UnityEngine.Object unityObject && unityObject == null)
            {
                Plugin.Logger.LogWarning($"Flow {FlowName} was cancelled because the target object was destroyed.");
                return;
            }

            await flow.ExecuteAsync(context, ct);
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
