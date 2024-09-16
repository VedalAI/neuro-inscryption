using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input.Helpers;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Input;

[HarmonyPatch]
public sealed class NeuroMouse : Singlyboi<NeuroMouse>
{
    public static async UniTask MoveToAsync(InteractableBase interactable, CancellationToken ct = default, float mouseSpeedMultiplier = 1, int tries = 10)
    {
        if (!interactable) throw new ArgumentNullException(nameof(interactable));

        Vector2 end = GetCursorPositionFor(interactable);

        while (Vector2.Distance(_simulatedMousePosition, end) > 0.1f)
        {
            _simulatedMousePosition = Vector2.MoveTowards(_simulatedMousePosition, end, mouseSpeedMultiplier * Time.deltaTime * 5 * Plugin.Config.MouseSpeedMultiplier.Value);
            end = GetCursorPositionFor(interactable);
            await UniTask.Yield(ct);
        }

        _simulatedMousePosition = end;
        await UniTask.Yield(ct);

        if (tries <= 0) return;

        InteractionCursor cursor = InteractionCursor.Instance;
        int layerMask = ~LayerMask.GetMask(cursor.excludedLayers.ToArray());
        InteractableBase? raycastHit = cursor.RaycastForInteractable<InteractableBase>(layerMask, cursor.transform.position);
        if (!CursorReachedTarget(interactable, raycastHit))
        {
            Plugin.Logger.LogError($"Failed to move mouse to interactable {interactable} (instead found {raycastHit}), retrying...");
            await MoveToAsync(interactable, ct, mouseSpeedMultiplier * 2, tries - 1);
        }
    }

    public static void ClickOnce(bool alt = false)
    {
        ClickHandler target = !alt ? _leftClickHandler : _rightClickHandler;
        target.Enqueue(true);
        target.Enqueue(false);
    }

    #region Position

    private static Vector2 _simulatedMousePosition;

    [HarmonyPatch(typeof(InteractionCursor), nameof(InteractionCursor.UpdatePosition))]
    [HarmonyPrefix]
    private static bool CustomPosition(InteractionCursor __instance)
    {
        Cursor.lockState = CursorLockMode.None;

        if (AutomationManager.Instance.AllowManualInput) return true;

        Cursor.visible = false;

        __instance.transform.position = new Vector3(_simulatedMousePosition.x, _simulatedMousePosition.y, __instance.rayCamera.nearClipPlane);

        return false;
    }

    [HarmonyPatch(typeof(InteractionCursor), nameof(InteractionCursor.UpdatePosition))]
    [HarmonyPostfix]
    private static void ResetSimulatedPosition(InteractionCursor __instance)
    {
        if (AutomationManager.Instance.AllowManualInput) _simulatedMousePosition = __instance.transform.position;
    }

    #endregion

    #region Clicking

    private static readonly ClickHandler _leftClickHandler = new();
    private static readonly ClickHandler _rightClickHandler = new();

    [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButton))]
    [HarmonyPrefix]
    private static bool CustomGetButton(ref bool __result, Button button)
    {
        if (AutomationManager.Instance.AllowManualInput) return true;
        if (button != Button.Select && button != Button.AltSelect) return true;

        __result = button == Button.Select ? _leftClickHandler.Get() : _rightClickHandler.Get();

        return false;
    }

    [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonDown))]
    [HarmonyPrefix]
    private static bool CustomGetButtonDown(ref bool __result, Button button)
    {
        if (AutomationManager.Instance.AllowManualInput) return true;
        if (button != Button.Select && button != Button.AltSelect) return true;

        __result = button == Button.Select ? _leftClickHandler.GetDown() : _rightClickHandler.GetDown();

        return false;
    }

    [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonUp))]
    [HarmonyPrefix]
    private static bool CustomGetButtonUp(ref bool __result, Button button)
    {
        if (AutomationManager.Instance.AllowManualInput) return true;
        if (button != Button.Select && button != Button.AltSelect) return true;

        __result = button == Button.Select ? _leftClickHandler.GetUp() : _rightClickHandler.GetUp();

        return false;
    }

    private void Update()
    {
        _leftClickHandler.Update();
        _rightClickHandler.Update();
    }

    #endregion

    private static bool CursorReachedTarget(InteractableBase target, InteractableBase? currentHit)
    {
        if (currentHit == null) return false;

        if (target is CardSlot slot1 && currentHit is PlayableCard card1) return slot1.Card == card1;
        if (target is PlayableCard card2 && currentHit is CardSlot slot2) return slot2.Card == card2;
        if (target is CardSlot slot3 && currentHit is BoneTokenInteractable bone) return bone.GetIdentifier().slot == slot3;

        return target == currentHit;
    }

    private static Vector3? GetPreferredWorldPositionFor(InteractableBase interactable)
    {
        if (interactable is CombatBell3D) return interactable.transform.position + Vector3.up * 0.5f;

        return null;
    }

    private static Vector2 GetCursorPositionFor(InteractableBase interactable)
    {
        Vector3? preferredPosition = GetPreferredWorldPositionFor(interactable);
        if (preferredPosition != null && IsPointOnScreen(preferredPosition.Value))
        {
            if (isOk(preferredPosition.Value, out Vector2 result)) return result;
        }

        if (IsPointOnScreen(interactable.transform.position))
        {
            if (isOk(interactable.transform.position, out Vector2 result)) return result;
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

        Plugin.Logger.LogWarning($"Cannot find a suitable cursor position for interactable {interactable}! Gonna move to the center, even if the raycast there hits {InteractionCursor.Instance.CastForInteractableAtWorldPosition(interactable.transform.position)}");
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
}
