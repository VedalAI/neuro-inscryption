using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NeuroInscryption.Communication.Actions;
using NeuroInscryption.Communication.Data.State;
using NeuroSdk.Actions;
using NeuroSdk.Internal;

namespace NeuroInscryption.Communication;

public sealed class AsyncActionWindow(string query, IGameState state)
{
    private readonly ActionWindow _window = ActionWindow.Create(Plugin.PluginObject);
    private readonly List<UniTask> _tasks = [];

    public UniTask SendAsync(CancellationToken ct = default)
    {
        _window.SetForce(1, () => query,() => Jason.Serialize(state));
        _window.Register();
        return UniTask.WhenAny(_tasks).AttachExternalCancellation(ct);
    }

    public AsyncActionWindow AddAction(IAsyncNeuroAction action)
    {
        _window.AddAction(action);
        _tasks.Add(action.ReturningTask);
        return this;
    }
}