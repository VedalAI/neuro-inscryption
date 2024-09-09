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
    public static void SnapTo(InteractableBase interactable)
    {
        _simulatedMousePosition = GetScreenPoint(interactable.transform.position);
    }

    public static IEnumerator MoveTo(InteractableBase interactable, float mouseSpeedMultiplier = 1)
    {
        if (_simulatedMousePosition == default)
        {
            SnapTo(interactable);
            yield break;
        }

        Vector2 end = GetScreenPoint(interactable.transform.position);

        while (Vector2.Distance(_simulatedMousePosition, end) > 0.1f)
        {
            _simulatedMousePosition = Vector2.MoveTowards(_simulatedMousePosition, end, mouseSpeedMultiplier * Time.deltaTime * 1750 * Plugin.Config.MouseSpeedMultiplier.Value);
            yield return null;
        }

        _simulatedMousePosition = end;
    }

    public static void SnapOffScreen()
    {
        _simulatedMousePosition = default;
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

        Vector2 worldPoint = __instance.rayCamera.ScreenToWorldPoint(_simulatedMousePosition);
        __instance.transform.position = new Vector3(worldPoint.x, worldPoint.y, __instance.rayCamera.nearClipPlane);

        return false;
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

    private static Vector2 GetScreenPoint(Vector3 position) => InteractionCursor.Instance.rayCamera.WorldToScreenPoint(position);
}
