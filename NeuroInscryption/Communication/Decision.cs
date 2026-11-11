using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Communication;

public record struct DecisionResult(string PickedAction, JToken? Data);

public sealed class Decision(string query, IGameState state)
{
    private readonly ActionWindow _actionWindow = ActionWindow.Create(Plugin.PluginObject); // scuff?

    public DecisionResult? RawResponse { get; private set; }

    private UniTaskCompletionSource<ActionBase> _tcs = new();
    public float Timeout { get; init; } = 1;
    public bool IsEphemeral { get; init; } = false;

    public void AddAction(ActionBase action)
    {
        _actionWindow.AddAction(new DecisionAction(this, action));
    }

    public UniTask<ActionBase> Send(CancellationToken ct)
    {
        if (Timeout >= 0) _actionWindow.SetForce(Timeout, () => query, () => MarkdownSerializer.Serialize(state), IsEphemeral);
        _actionWindow.SetEnd(() => ct.IsCancellationRequested);
        _actionWindow.Register();
        return _tcs.Task;
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
                if (!ourAction.Validate(decision))
                    return ExecutionResult.Failure($"Action failed validation. {ActionStrings.CheckDataMsg}");
                return ExecutionResult.Success();
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"Failed to validate {Name} in decision");
                Plugin.Logger.LogError(e);
                return ExecutionResult.ModFailure($"Failed to validate action {Name}");
            }
        }

        protected override void Execute()
        {
            try
            {
                // this synchronously runs any continuations (i.e. calls back into awaiters)
                decision._tcs.TrySetResult(ourAction);
            }
            finally
            {
                decision._tcs = new UniTaskCompletionSource<ActionBase>();
            }
        }
    }
}
