using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class ChoosePathFlow : AutomationFlow
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class Context
    {
        public readonly IReadOnlyList<IReadOnlyList<string>> Paths;
        public readonly IReadOnlyDictionary<string, string> PathDescriptions;

        public Context(List<List<MapNodeData>> paths)
        {
            Paths = paths.Select(path => path.Select(node => node.Name).ToList()).ToList();
            PathDescriptions = paths.Flatten().ToDescribableDictionary();
        }
    }

    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.Method(typeof(MapNodeManager), nameof(MapNodeManager.SetActiveNode))
        }
    ];

    public override async UniTask ExecuteAsync(HarmonyContext context, CancellationToken ct)
    {
        await UniTask.Delay(1000, cancellationToken: ct);

        List<List<MapNodeData>> paths = GetPaths(context.GetInstance<MapNodeManager>()!.ActiveNode);
        Dictionary<string, MapNodeData> nextNodes = paths.Select(p => p.First()).Distinct()
            .Select((node, index) => (desc: node.Name + string.Format(ActionStrings.PathDiscriminator, index), node))
            .ToDictionary(tuple => tuple.desc, tuple => tuple.node);

        LoadoutState state = LoadoutState.Calculate(new Context(paths));

        ChoosePathAction choosePathAction = new(nextNodes.Keys);

        Decision decision = new(DecisionIds.PathChoice, state, DecisionStrings.ChoosePath);
        decision.AddAction(choosePathAction);

        Action action = await decision.SendAsync(ct);

        if (action.Match(choosePathAction, out string? path))
        {
            await NeuroMouse.MoveToAsync(nextNodes[path].MapNode, 500, ct);
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
