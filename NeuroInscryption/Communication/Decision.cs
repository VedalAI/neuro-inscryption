using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.State;
using NeuroSdk.Actions;
using NeuroSdk.Internal;
using NeuroSdk.Json;
using NeuroSdk.Messages.Outgoing;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication;

public record struct DecisionResult(string PickedAction, JToken? Data);

public sealed class Decision(string query, IGameState state)
{
    private readonly ActionWindow _actionWindow = ActionWindow.Create(Plugin.PluginObject); // scuff?

    public DecisionResult? RawResponse { get; private set; }
    public ExecutionResult? Result { get; private set; }

    private UniTaskCompletionSource _tcs = new();
    public float Timeout { get; init; } = 1;
    public bool IsEphemeral { get; init; } = false;

    public void AddAction(ActionBase action)
    {
        _actionWindow.AddAction(new DecisionAction(this, action));
    }

    public UniTask Send(CancellationToken ct)
    {
        // todo: better state representation
        if (Timeout >= 0) _actionWindow.SetForce(Timeout, () => query, () => Jason.Serialize(state), IsEphemeral);
        _actionWindow.SetEnd(() => ct.IsCancellationRequested);
        _actionWindow.Register();
        return _tcs.Task;
    }

    private const string CheckDataMsg = "It is possible that you provided invalid data to an action. Please try again and ensure that the data conforms to the given schema.";

    [DoesNotReturn]
    public void ThrowInvalid()
    {
        // this shouldn't ever get called anymore
        AutomationManager.Assert(RawResponse.HasValue, "ThrowInvalid without Result");
        string errMsg = $"Received action '{RawResponse.Value.PickedAction}' which did not match possible expected actions.";
        Context.Send($"{errMsg} {CheckDataMsg}");
        throw new InvalidOperationException(errMsg); // restarts the flow
    }

    private sealed class DecisionAction(Decision decision, ActionBase ourAction) : NeuroAction
    {
        public override string Name => ourAction.Name;
        protected override string Description => ourAction.Description;
        public override bool CanAddToActionWindow(ActionWindow window) => ourAction.IsApplicable;

        protected override JsonSchema? Schema => ourAction.GetDataSchema();
        protected override ExecutionResult Validate(ActionJData actionData)
        {
            decision.RawResponse = new(Name, actionData.Data);
            try
            {
                if (!ourAction.Matches(decision))
                    return ExecutionResult.Failure($"Action failed validation. {CheckDataMsg}");
                return ExecutionResult.Success();
            }
            catch (Exception e)
            {
                decision.Result = ExecutionResult.ModFailure($"Failed to validate action {Name}");
                Plugin.Logger.LogWarning($"Failed to validate {Name} in decision");
                Plugin.Logger.LogError(e);
                return decision.Result;
            }
        }

        protected override void Execute()
        {
            try
            {
                decision._tcs.TrySetResult(); // this synchronously runs any continuations (i.e. calls back into awaiters)
            }
            finally
            {
                decision._tcs = new UniTaskCompletionSource();
            }
        }
    }
}
