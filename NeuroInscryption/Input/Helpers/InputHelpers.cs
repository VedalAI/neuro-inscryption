using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;

namespace NeuroInscryption.Input.Helpers;
internal static class InputHelpers
{
    public static UniTask WaitUntilViewLocked(CancellationToken ct = default)
        => UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Locked, cancellationToken: ct);
    public static UniTask WaitUntilViewUnlocked(CancellationToken ct = default)
        => UniTask.WaitUntil(() => ViewManager.Instance.Controller.LockState == ViewLockState.Unlocked, cancellationToken: ct);
}
