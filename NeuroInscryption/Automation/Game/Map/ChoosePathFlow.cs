using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Input;
using NeuroInscryption.Input.Helpers;
using UnityEngine;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ChoosePathFlow : HarmonyAutomationFlow<MapNodeManager>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(MapNodeManager), nameof(MapNodeManager.SetActiveNode))
        }
    ];

    protected override async UniTask ExecuteAsync(MapNodeManager instance, CancellationToken ct)
    {
        // don't send decision after manually picking node (also fixes decision after area boss)
        if (!instance.ActiveNode || !instance.ActiveNode.connectedNodes.Any(IsActive))
        {
            Plugin.Logger.LogMessage($"No active nodes, cancelling {nameof(ChoosePathFlow)}");
            return;
        }

        MapNodeData node = await Neuro.Map.ChooseNextNode(ct);

        await InputHelpers.WaitUntilViewUnlocked(ct);
        await NeuroMouse.MoveToAsync(node.MapNode, 300, ct);
        await NeuroMouse.ClickAsync(ct);
    }

    private static bool IsActive(MapNode node)
    {
        return node && node.isActiveAndEnabled && node.GetComponent<BoxCollider>().enabled; // yep
    }
}
