using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using JetBrains.Annotations;

namespace NeuroInscryption.Utilities;

[PublicAPI]
public static class Wait
{
    /// <summary>
    /// The time it takes for the last card in a card pile to be created.<br/>
    /// Does not include the time it takes for the last card to move towards its target position.
    /// </summary>
    /// <remarks>After this amount of time passes, the last card may still be offscreen.</remarks>
    public const int CardPileMinSpawnTimeMs = (int)(1000 * (CardPile.SPAWN_TIME - CardPile.SPAWN_TWEEN_LENGTH));
    /// <summary>
    /// The time it takes for the last card in a card pile to be created and move towards its target position.
    /// </summary>
    /// <remarks>After this amount of time passes, all cards are guaranteed to be arranged on the board.</remarks>
    public const int CardPileFullSpawnTimeMs = (int)(1000 * CardPile.SPAWN_TIME);

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

    public static UniTask ViewLocked(CancellationToken ct = default)
    {
        return View(ViewLockState.Locked, ct);
    }

    public static UniTask ViewUnlocked(CancellationToken ct = default)
    {
        return View(ViewLockState.Unlocked, ct);
    }

    public static UniTask CameraStable(CancellationToken ct = default)
    {
        return WaitUntilPossiblySync(() => CameraStabilityTracker.Instance.IsStable, ct);
    }

    private static UniTask WaitUntilPossiblySync(Func<bool> predicate, CancellationToken ct = default)
    {
        // i'm not 500% sure on this but i think UniTask yields at least once just like unity
        // whereas i want the corlib task behaviour (complete synchronously if possible)
        return predicate() ? UniTask.CompletedTask
            : UniTask.WaitUntil(predicate, cancellationToken: ct);
    }

    // todo: move this maybe
    // also desperately needs a better name
    // names already discarded:
    //   - SceneChanged
    //   - FlowStarted
    //   - ReactionTime
    // this is like a "reorienting" time, sort of like how visual acquisition after a view shift takes a little bit
    public static async UniTask CommonFlowDelay(CancellationToken ct = default)
    {
        // await ViewUnlocked(ct); // some nodes intentionally lock view
        await CameraStable(ct);
        // current thinking is to make a central source for randomized "human-like" "reaction time"
        // as it is commonly known, human = normal distribution
        await UniTask.Delay(300, cancellationToken: ct);
    }
}
