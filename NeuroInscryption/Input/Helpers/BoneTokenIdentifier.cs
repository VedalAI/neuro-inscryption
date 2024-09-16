using System.Collections;
using System.Linq;
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
    public static void AddIdentifierComponent(object __instance, bool __result)
    {
        if (__instance.GetEnumeratorState() != 1 || __result == false) return;

        Part1ResourcesManager instance = __instance.GetEnumeratorThis<Part1ResourcesManager>();
        CardSlot slot = __instance.GetEnumeratorField<CardSlot>("slot");

        BoneTokenIdentifier id = instance.boneTokens.Last().gameObject.AddComponent<BoneTokenIdentifier>();
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
