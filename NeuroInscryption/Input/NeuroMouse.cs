using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Input;

[HarmonyPatch]
public sealed class NeuroMouse : Singlyboi<NeuroMouse>
{
    public static IEnumerator MoveTo(InteractableBase interactable, float mouseSpeedMultiplier = 1)
    {
        Vector2 end = GetCursorPosition(interactable);

        while (Vector2.Distance(_simulatedMousePosition, end) > 0.1f)
        {
            if (AutomationManager.Instance.AllowManualInput) yield break;

            _simulatedMousePosition = Vector2.MoveTowards(_simulatedMousePosition, end, mouseSpeedMultiplier * Time.deltaTime * 5 * Plugin.Config.MouseSpeedMultiplier.Value);
            yield return null;
        }

        _simulatedMousePosition = end;

        if (InteractionCursor.Instance.RaycastForInteractable<InteractableBase>(~LayerMask.GetMask(InteractionCursor.Instance.excludedLayers.ToArray()), InteractionCursor.Instance.transform.position) != interactable)
        {
            Plugin.Logger.LogError("Failed to move mouse to interactable, retrying...");
            yield return MoveTo(interactable, mouseSpeedMultiplier * 2);
        }
    }

    public static void ClickOnce(bool alt = false)
    {
        StartHolding(alt);
        StopHolding(alt);
    }

    public static void StartHolding(bool alt = false)
    {
        ClickHandler target = !alt ? _leftClickHandler : _rightClickHandler;

        target.Enqueue(true);
    }

    public static void StopHolding(bool alt = false)
    {
        ClickHandler target = !alt ? _leftClickHandler : _rightClickHandler;

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

    private static Vector2 GetCursorPosition(InteractableBase interactable)
    {
        Vector3 position;

        Transform topLeft = interactable.transform.Find("TopLeftMarker");
        Transform topRight = interactable.transform.Find("TopRightMarker");

        if (topLeft && InteractionCursor.Instance.CastForInteractableAtWorldPosition(topLeft.position) == interactable) position = topLeft.position;
        else if (topRight) position = topRight.position;
        else position = interactable.transform.position;

        Vector2 screenPoint = Camera.main!.WorldToScreenPoint(position);
        Vector2 cursorPoint = InteractionCursor.Instance.rayCamera.ScreenToWorldPoint(screenPoint);
        return cursorPoint;
    }
}
