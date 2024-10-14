using System.Collections;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Utilities;
using UnityEngine;

namespace NeuroInscryption.Input.Helpers;

[HarmonyPatch]
public sealed class BoneTokenIdentifier : MonoBehaviour
{
    public CardSlot? slot;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        slot = null;
    }

    [HarmonyPatch(typeof(Part1ResourcesManager), nameof(Part1ResourcesManager.ShowAddBones), MethodType.Enumerator)]
    [HarmonyPostfix]
    public static void AddIdentifierComponent(IEnumerator __instance, bool __result)
    {
        if (!__result) return;
        EnumeratorWrapper<Part1ResourcesManager> wrapper = new(__instance);
        if (wrapper.State != 1) return;

        Part1ResourcesManager instance = wrapper.This;
        CardSlot slot = wrapper.GetFieldValue<CardSlot>("slot");

        BoneTokenIdentifier id = instance.boneTokens[^1].gameObject.AddComponent<BoneTokenIdentifier>();
        id.slot = slot;
    }
}

public static class BoneTokenIdentifierExtensions
{
    public static BoneTokenIdentifier GetIdentifier(this BoneTokenInteractable token)
    {
        return token.GetComponent<BoneTokenIdentifier>();
    }
}
