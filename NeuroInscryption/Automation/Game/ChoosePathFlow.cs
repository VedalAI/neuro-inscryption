using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data;

namespace NeuroInscryption.Automation.Game;

public sealed class ChoosePathFlow : AutomationFlow
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(MapNodeManager), nameof(MapNodeManager.SetActiveNode))
        }
    ];

    public override async UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct)
    {
        MapNodeManager instance = context.GetInstance<MapNodeManager>()!;

        await UniTask.Delay(1000, cancellationToken: ct);

        List<List<MapNodeData>> paths = GetPaths(instance.ActiveNode);
    }

    private static List<List<MapNodeData>> GetPaths(MapNode node, int depth = 0)
    {
        // TODO: this should be state

        if (node.connectedNodes.Count == 0 || depth == 5)
        {
            return [[node.Data]];
        }

        List<List<MapNodeData>> result = [];

        foreach (MapNode childNode in node.connectedNodes)
        {
            List<List<MapNodeData>> paths = GetPaths(childNode, depth + 1);

            if (depth != 0)
            {
                foreach (List<MapNodeData> t in paths)
                {
                    t.Insert(0, node.Data);
                }
            }

            result.AddRange(paths);
        }

        return result;
    }
}
