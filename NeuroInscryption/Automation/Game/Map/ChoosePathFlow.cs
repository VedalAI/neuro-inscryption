using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ChoosePathFlow : AutomationFlow<MapNodeManager>
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context(List<List<MapNodeData>> paths)
    {
        public readonly IReadOnlyList<string> Paths = paths.Select(path => string.Join(" -> ", path.Select(node => node.Identifier))).ToList();
        public readonly IReadOnlyDictionary<string, string> PathDescriptions = paths.Flatten().ToDescriptionDictionary();
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(MapNodeManager), nameof(MapNodeManager.SetActiveNode))
        }
    ];

    protected override async UniTask ExecuteAsync(MapNodeManager instance, CancellationToken ct)
    {
        await UniTask.Delay(1000, cancellationToken: ct);

        List<List<MapNodeData>> paths = GetPaths(instance.ActiveNode);
        List<MapNodeData> nextNodes = paths.Select(p => p.First()).Distinct().ToList();

        LoadoutState state = LoadoutState.CalculateWithContext(new Context(paths));

        ChooseAction<MapNodeData> choosePathAction = new(nextNodes, true);

        Decision decision = new(DecisionStrings.ChoosePath, state);
        decision.AddAction(choosePathAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(choosePathAction, out MapNodeData? node))
        {
            await NeuroMouse.MoveToAsync(node.MapNode, 300, ct);
            await NeuroMouse.ClickAsync(ct);
        }
        else
        {
            action.ThrowInvalid();
        }
    }

    private static List<List<MapNodeData>> GetPaths(MapNode node, int depth = 0)
    {
        if (node.connectedNodes.Count == 0 || depth == 5)
        {
            return [[node]];
        }

        List<List<MapNodeData>> result = [];

        foreach (MapNode childNode in node.connectedNodes)
        {
            List<List<MapNodeData>> paths = GetPaths(childNode, depth + 1);

            if (depth != 0)
            {
                foreach (List<MapNodeData> t in paths)
                {
                    t.Insert(0, node);
                }
            }

            result.AddRange(paths);
        }

        return result;
    }
}
