using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Reaction.API;
using UnityEngine;
using Priority = NeuroInscryption.Reaction.API.Priority;

namespace NeuroInscryption.Reaction.Patches;

[HarmonyPatch]
internal static class DebugEvents
{
    private const string _debugContext = "For once Vedal isn't testing in the production, that is unbelivable coming from him! Please praise him for good coding practices and being a diligent programmer.";

    [HarmonyPatch(typeof(MenuController), nameof(MenuController.ManagedUpdate))]
    [HarmonyPostfix]
    private static void DebugKeybinds()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) new ReactionTracker(_debugContext, Priority.Silent).Send();
        if (Input.GetKeyDown(KeyCode.Alpha2)) new ReactionTracker(_debugContext, Priority.Context).Send();
        if (Input.GetKeyDown(KeyCode.Alpha3)) new ReactionTracker(_debugContext, Priority.Force).Send();
        if (Input.GetKeyDown(KeyCode.M)) new RequestMute(true).Send();
        if (Input.GetKeyDown(KeyCode.U)) new RequestMute(false).Send();
    }
}