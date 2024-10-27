using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using JetBrains.Annotations;

namespace NeuroInscryption.Utilities;

[PublicAPI]
public static class Wait
{
    private const int CardPileSpawnTimeMs = (int)(1000 * (CardPile.SPAWN_TIME - CardPile.SPAWN_TWEEN_LENGTH));

    public static async UniTask View(View targetView, CancellationToken ct = default, bool stable = true, bool unlocked = false)
    {
        await WaitUntilPossiblySync(() => ViewManager.Instance.Is(targetView), ct);
        if (stable) await CameraStable(ct);
        if (unlocked) await ViewUnlocked(ct);
    }

    public static UniTask View(ViewLockState targetLockState, CancellationToken ct = default)
    {
        return WaitUntilPossiblySync(() => ViewManager.Instance.Is(targetLockState), ct);
    }
    public static UniTask ViewLocked(CancellationToken ct = default) => View(ViewLockState.Locked, ct);
    public static UniTask ViewUnlocked(CancellationToken ct = default) => View(ViewLockState.Unlocked, ct);


    public static UniTask CameraStable(CancellationToken ct = default)
    {
        return WaitUntilPossiblySync(() => CameraStabilityTracker.Instance.IsStable, ct);
    }

    public static UniTask CardPileSpawned(CancellationToken ct = default)
    {
        return UniTask.Delay(CardPileSpawnTimeMs, cancellationToken: ct);
    }

    private static UniTask WaitUntilPossiblySync(Func<bool> predicate, CancellationToken ct = default)
    {
        // i'm not 500% sure on this but i think UniTask waits at least 1 yield just like unity
        // whereas i want the corlib task behaviour (complete synchronously if possible)
        return predicate() ? UniTask.CompletedTask
            : UniTask.WaitUntil(predicate, cancellationToken: ct);
    }
}
