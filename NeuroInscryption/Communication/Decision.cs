using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication;

public sealed class Decision(string id, IGameState state, string query)
{
    private ActionWindow ActionWindow { get; } = ActionWindow.Create(Plugin.PluginObject);

    public string? PickedAction { get; private set; }
    public JToken? Data { get; private set; }

    private UniTaskCompletionSource _tcs = new();
    public float Timeout { get; set; } = 5;

    [SetsRequiredMembers]
    public Decision(Description decisionDescription, IGameState state) : this(decisionDescription.Name, state, decisionDescription.Info)
    {
    }

    public void AddAction(ActionBase action)
    {
        ActionWindow.AddAction(new DecisionAction(ActionWindow, this, action));
    }

    public UniTask Send(CancellationToken ct)
    {
        // todo: serialize state
        if (Timeout > 0) ActionWindow.SetForce(Timeout, () => query, state.ToString);
        ActionWindow.SetEnd(() => ct.IsCancellationRequested);
        ActionWindow.Register();
        return _tcs.Task;
    }

    [DoesNotReturn]
    public void ThrowInvalid()
    {
        throw new InvalidOperationException($"Received action '{PickedAction}' which did not match possible expected actions.");
    }

    private sealed class DecisionAction(ActionWindow? actionWindow, Decision decision, ActionBase ourAction)
        : NeuroAction(actionWindow)
    {
        public override string Name => ourAction.Name;
        protected override string Description => "todo";
        public override bool CanBeUsed() => ourAction.IsApplicable;

        protected override JsonSchema? Schema => ourAction.GetDataSchema();
        protected override ExecutionResult Validate(ActionJData actionData)
        {
            try
            {
                decision.Data = actionData.Data;
                decision.PickedAction = Name;
                return ExecutionResult.Success();
            }
            catch
            {
                return ExecutionResult.Failure("Something went wrong."); // placeholder
            }
        }

        protected override UniTask ExecuteAsync()
        {
            try
            {
                decision._tcs.TrySetResult();
            }
            finally
            {
                decision._tcs = new();
            }
            return UniTask.CompletedTask;
        }
    }

}
