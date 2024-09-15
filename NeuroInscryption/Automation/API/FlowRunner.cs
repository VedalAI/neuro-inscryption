using System;
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
        Plugin.Logger.LogMessage($"Changing flow from {FlowName} to {flow.GetType().Name}.");

        _cts?.Cancel();

        _currentFlow = flow;
        _currentContext = context;
    }

    public void RestartLastFlow()
    {
        if (_currentFlow == null) return;

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();
        ExecuteFlowAsync(_currentFlow, _currentContext!, _cts.Token).Forget();
    }

    private async UniTaskVoid ExecuteFlowAsync(AutomationFlow flow, HarmonyContext context, CancellationToken ct)
    {
        try
        {
            await flow.ExecuteAsync(context, ct);
            Plugin.Logger.LogMessage($"Flow {FlowName} completed.");
            _currentFlow = null;
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Caught exception while executing flow {FlowName}. Flow will be restarted");
            Plugin.Logger.LogError(e);
            RestartLastFlow();
        }
    }
}
