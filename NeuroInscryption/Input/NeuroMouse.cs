using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Input.Helpers;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using UnityEngine;

namespace NeuroInscryption.Input;

public static class NeuroMouse
{
    private const float MouseSpeed = 4.0f; // magic number, works well with the cursor's weird coordinate scale

    private static InteractableBase? _current;

    public static async UniTask MoveTo(InteractableBase interactable, CancellationToken ct = default, float mouseSpeedMultiplier = 1, int tries = 10)
    {
        if (!interactable) throw new ArgumentNullException(nameof(interactable));
        if (_current != null && _current != interactable)
        {
            Plugin.Logger.LogWarning($"Tried to move mouse to {interactable} while something else is already moving it to {_current}");
            return;
        }

        _current = interactable;
        try
        {
            await inner();
        }
        finally
        {
            _current = null;
        }
        return;

        async Task inner()
        {
            Vector2 end = GetCursorPositionFor(interactable);
            float distance = Vector2.Distance(InputHandler.SimulatedMousePosition, end);
            float duration = Mathf.Sqrt(distance) / (mouseSpeedMultiplier * MouseSpeed);
            // Plugin.Logger.LogWarning($"Move {distance} at {mouseSpeedMultiplier}x{MouseSpeed}, should take {duration}");

            bool complete = false;
            ValueVector2 tween = default!;
            tween = (ValueVector2)Tween.Value(InputHandler.SimulatedMousePosition, end,
                valueUpdatedCallback: curr =>
                {
                    InputHandler.SimulatedMousePosition = curr;
                    // ReSharper disable once AccessToModifiedClosure
                    if (ct.IsCancellationRequested) tween.Stop();
                },
                duration: duration,
                easeCurve: Tween.EaseOut,
                delay: 0,
                completeCallback: () => complete = true
            );

            while (!complete)
            {
                tween.EndValue = GetCursorPositionFor(interactable, false);
                await UniTask.Yield(ct);
            }
            if (tries <= 0) return;

            InteractionCursor cursor = InteractionCursor.Instance;
            int layerMask = ~LayerMask.GetMask(cursor.excludedLayers.ToArray());
            InteractableBase? raycastHit = cursor.RaycastForInteractable<InteractableBase>(layerMask, cursor.transform.position);
            if (!CursorReachedTarget(interactable, raycastHit))
            {
                Plugin.Logger.LogError($"Failed to move mouse to interactable {interactable} (instead found {raycastHit}), retrying...");
                await MoveTo(interactable, ct, mouseSpeedMultiplier * 2, tries - 1);
            }
        }
    }

    public static async UniTask MoveTo(InteractableBase interactable, int msDelay, CancellationToken ct = default, float mouseSpeedMultiplier = 1, int tries = 10)
    {
        await MoveTo(interactable, ct, mouseSpeedMultiplier, tries);
        await UniTask.Delay(msDelay, cancellationToken: ct);
    }

    public static async UniTask Click(CancellationToken ct = default, bool alt = false)
    {
        Button button = !alt ? Button.Select : Button.AltSelect;
        InputHandler.EnqueueInput(button, true);
        // release if cancelled
        try
        {
            await UniTask.Delay(85, cancellationToken: ct);
        }
        finally
        {
            InputHandler.EnqueueInput(button, false);
        }
    }

    public static async UniTask Click(int msDelay, CancellationToken ct = default, bool alt = false)
    {
        await Click(ct, alt);
        await UniTask.Delay(msDelay, cancellationToken: ct);
    }

    /// <summary>
    /// Move the mouse to an <see cref="InteractableBase">interactable</see> <paramref name="target"/> and click it.
    /// </summary>
    /// <param name="target">If the interactable is not <see cref="InteractableBase.Enabled">enabled</see>, this function will wait up to <paramref name="waitTargetEnabledMsTimeout"/> milliseconds before trying to click anyway.</param>
    /// <param name="msDelayBeforeClick">Delay between moving the mouse and clicking.</param>
    /// <param name="waitTargetEnabledMsTimeout">If above zero and the target is not <see cref="InteractableBase.Enabled">enabled</see>, this function will wait up to this timeout (in milliseconds) before trying to click anyway.</param>
    /// <param name="alt">Right click instead of left clicking.</param>
    public static async UniTask MoveAndClick(InteractableBase target, CancellationToken ct = default,
        int msDelayBeforeClick = 200, int msDelayAfterClick = 100,
        float mouseSpeedMultiplier = 1f, int waitTargetEnabledMsTimeout = 1000,
        bool alt = false)
    {
        if (!await WaitTargetEnabled(target, ct, waitTargetEnabledMsTimeout)) return; // todo: decide how hard to crash here

        await MoveTo(target, ct, mouseSpeedMultiplier);
        if (msDelayBeforeClick > 0) await UniTask.Delay(msDelayBeforeClick, cancellationToken: ct);
        await Click(ct, alt);
        if (msDelayAfterClick > 0) await UniTask.Delay(msDelayAfterClick, cancellationToken: ct);
    }

    private static async UniTask<bool> WaitTargetEnabled(InteractableBase target, CancellationToken ct, int msTimeout = 1000)
    {
        if (!target)
        {
            Plugin.Logger.LogError($"Mouse target {target} is null/dead");
            return false;
        }
        if (msTimeout <= 0 || target.Enabled)
            return true;

        Plugin.Logger.LogWarning($"Interactable {target} is disabled, gonna wait for a bit but then you're on your own");
        CancellationTokenSource timeoutCts = new(msTimeout);
        using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, ct))
        {
            await UniTask.WaitUntil(() => !target || target.Enabled, cancellationToken: linkedCts.Token)
                .SuppressCancellationThrow();
        }
        ct.ThrowIfCancellationRequested(); // only suppress our inner timeout

        if (timeoutCts.IsCancellationRequested)
            Plugin.Logger.LogError($"Timed out waiting for interactable {target} to become enabled, gonna try to click it anyway");

        if (target) return true;

        Plugin.Logger.LogError($"Interactable {target} died while we were waiting for it to become enabled");
        return false;
    }

    #region Movement shenanigans

    private static bool CursorReachedTarget(InteractableBase target, InteractableBase? currentHit)
    {
        if (currentHit == null) return false;

        if (target is CardSlot slot1 && currentHit is PlayableCard card1) return slot1.Card == card1;
        if (target is PlayableCard card2 && currentHit is CardSlot slot2) return slot2.Card == card2;
        if (target is CardSlot slot3 && currentHit is BoneTokenInteractable bone3) return bone3.GetIdentifier().slot == slot3;
        if (target is SelectCardFromDeckSlot slot4 && currentHit is SelectableCard card4) return slot4.Card == card4;
        if (target is SelectableCard card5 && currentHit is SelectCardFromDeckSlot slot5) return slot5.Card == card5;

        return target == currentHit;
    }

    private static List<Vector3> GetPreferredWorldPositionsFor(InteractableBase interactable)
    {
        if (interactable is CombatBell3D) return [interactable.transform.position + Vector3.up * 0.5f];
        if (interactable is SelectableItemSlot or ConsumableItemSlot) return [interactable.transform.position + Vector3.up];
        if (interactable is SelectableCard) return [interactable.transform.position, interactable.transform.position + new Vector3(-0.5f, 0)];

        return [];
    }

    /// <summary>
    /// Searches for a suitable position for the cursor.
    /// If no good position is found, returns the center of the <paramref name="interactable"/>.
    /// </summary>
    /// <param name="warn">Log a warning if there's no good position available.</param>
    private static Vector2 GetCursorPositionFor(InteractableBase interactable, bool warn = true)
    {
        List<Vector3> worldPositions = GetPreferredWorldPositionsFor(interactable);
        worldPositions.Add(interactable.transform.position);

        foreach (Vector3 position in worldPositions)
        {
            if (IsPointOnScreen(position) && isOk(position, out Vector2 result)) return result;
        }

        Transform topLeft = interactable.transform.Find("TopLeftMarker");
        Transform topRight = interactable.transform.Find("TopRightMarker");

        if (topLeft && topRight)
        {
            Vector3 averagePosition = (topLeft.position + topRight.position) / 2;
            if (isOk(averagePosition, out Vector2 result)) return result;
        }

        if (topLeft)
        {
            if (isOk(topLeft.position, out Vector2 result)) return result;
        }

        if (topRight)
        {
            if (isOk(topRight.position, out Vector2 result)) return result;
        }

        MainInputInteractable? atCenter = InteractionCursor.Instance.CastForInteractableAtWorldPosition(interactable.transform.position);
        if (atCenter && atCenter != interactable && warn)
        {
            Plugin.Logger.LogWarning($"Cannot find a suitable cursor position for interactable {interactable}! Gonna move to the center, even if the raycast there hits {atCenter}");
        }
        return GetCursorPointForWorldPoint(interactable.transform.position);

        bool isOk(Vector3 worldPosition, out Vector2 cursorPoint)
        {
            if (CursorReachedTarget(interactable, InteractionCursor.Instance.CastForInteractableAtWorldPosition(worldPosition)))
            {
                cursorPoint = GetCursorPointForWorldPoint(worldPosition);
                return true;
            }

            cursorPoint = default;
            return false;
        }
    }

    private static Vector2 GetCursorPointForWorldPoint(Vector3 worldPosition)
    {
        Vector2 screenPoint = Camera.main!.WorldToScreenPoint(worldPosition);
        Vector2 cursorPoint = InteractionCursor.Instance.rayCamera.ScreenToWorldPoint(screenPoint);
        return cursorPoint;
    }

    private static bool IsPointOnScreen(Vector3 worldPosition)
    {
        Vector3 screenPoint = Camera.main!.WorldToViewportPoint(worldPosition);
        return screenPoint.x is > 0 and < 1 && screenPoint.y is > 0 and < 1;
    }

    #endregion
}
