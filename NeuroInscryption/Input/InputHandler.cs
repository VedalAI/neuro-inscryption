using System;
using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Input;

[HarmonyPatch]
public sealed class InputHandler : Singlyboi<InputHandler>
{
    #region Cursor position

    public static Vector2 SimulatedMousePosition { get; set; }

    [HarmonyPatch(typeof(InteractionCursor), nameof(InteractionCursor.UpdatePosition))]
    [HarmonyPrefix]
    private static bool CustomPosition(InteractionCursor __instance)
    {
        Cursor.lockState = CursorLockMode.None;

        if (AutomationManager.Instance.AllowManualInput) return true;

        Cursor.visible = false;

        __instance.transform.position = new Vector3(SimulatedMousePosition.x, SimulatedMousePosition.y, __instance.rayCamera.nearClipPlane);

        return false;
    }

    [HarmonyPatch(typeof(InteractionCursor), nameof(InteractionCursor.UpdatePosition))]
    [HarmonyPostfix]
    private static void ResetSimulatedPosition(InteractionCursor __instance)
    {
        if (AutomationManager.Instance.AllowManualInput) SimulatedMousePosition = __instance.transform.position;
    }

    #endregion

    #region Button handling

    private static readonly Dictionary<Button, IndividualButtonHandler> _buttonHandlers = Enum.GetValues(typeof(Button)).Cast<Button>()
        .ToDictionary(b => b, _ => new IndividualButtonHandler());

    public static void EnqueueInput(Button button, bool down)
    {
        _buttonHandlers[button].Enqueue(down);
    }

    [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButton))]
    [HarmonyPrefix]
    private static bool CustomGetButton(ref bool __result, Button button)
    {
        if (AutomationManager.Instance.AllowManualInput) return true;

        if (_buttonHandlers.TryGetValue(button, out IndividualButtonHandler? handler))
        {
            __result = handler.Get();
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonDown))]
    [HarmonyPrefix]
    private static bool CustomGetButtonDown(ref bool __result, Button button)
    {
        if (AutomationManager.Instance.AllowManualInput) return true;

        if (_buttonHandlers.TryGetValue(button, out IndividualButtonHandler? handler))
        {
            __result = handler.GetDown();
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(InputButtons), nameof(InputButtons.GetButtonUp))]
    [HarmonyPrefix]
    private static bool CustomGetButtonUp(ref bool __result, Button button)
    {
        if (AutomationManager.Instance.AllowManualInput) return true;

        if (_buttonHandlers.TryGetValue(button, out IndividualButtonHandler? handler))
        {
            __result = handler.GetUp();
            return false;
        }

        return true;
    }

    // TODO: fix wasd and scroll wheel

    private void Update()
    {
        foreach (IndividualButtonHandler handler in _buttonHandlers.Values)
        {
            handler.Update();
        }
    }

    #endregion
}
