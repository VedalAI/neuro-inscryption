using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;
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
        // don't send choice after already picked node (also fixes sending decision after area boss)
        if (!instance.ActiveNode || !instance.ActiveNode.connectedNodes.Any(IsActive))
        {
            Plugin.Logger.LogMessage($"No active nodes, exiting {nameof(ChoosePathFlow)}");
            return;
        }

        await Wait.CommonFlowDelay(ct);

        MapNodeData node = await Neuro.Map.ChooseNextNode(ct);

        await Wait.ViewUnlocked(ct);
        await NeuroMouse.MoveAndClick(node.MapNode, ct);
    }

    private static bool IsActive(MapNode node)
    {
        return node && node.isActiveAndEnabled && node.GetComponent<BoxCollider>().enabled; // yep
    }
}
