using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Actions.Map;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Messages.Outgoing;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation;
internal class Map
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class MapNodeContext(List<List<MapNodeData>> paths) : ILoadoutContext
    {
        public readonly IReadOnlyList<string> Paths = paths.ConvertAll(path => string.Join(" -> ", path.Select(node => node.Identifier)));
        public readonly IReadOnlyDictionary<string, string> PathDescriptions = paths.Flatten().ToDescriptionDictionary();

        IEnumerable<SigilData> ILoadoutContext.RelevantSigils => [];
        IEnumerable<TraitData> ILoadoutContext.RelevantTraits => [];
    }

    public async UniTask<MapNodeData> ChooseNextNode(CancellationToken ct)
    {
        MapNodeManager instance = MapNodeManager.Instance;
        List<List<MapNodeData>> paths = GetPaths(instance.ActiveNode);
        List<MapNodeData> nextNodes = paths.Select(p => p[0]).Distinct().ToList();

        if (nextNodes.Count == 1)
        {
            Plugin.Logger.LogDebug($"Only one path ({nextNodes[0].MapNode}), not gonna bother asking");
            return nextNodes[0];
        }

        LoadoutState state = LoadoutState.Calculate();
        state.SetContext(new MapNodeContext(paths));

        ChooseAction<MapNodeData> choosePathAction = new(nextNodes);

        Decision decision = new(DecisionStrings.ChoosePath, state);
        decision.AddAction(choosePathAction);

        Action action = await decision.SendAsync(ct);

        if (!action.Match(choosePathAction, out MapNodeData? node)) action.ThrowInvalid();

        return node;
    }

    private const int MaxDepth = 5;
    private static List<List<MapNodeData>> GetPaths(MapNode node, int depth = 0)
    {
        if (node.connectedNodes.Count == 0 || depth == MaxDepth)
        {
            return [[MapNodeData.Create(node)]];
        }

        List<List<MapNodeData>> result = [];

        foreach (MapNode childNode in node.connectedNodes)
        {
            List<List<MapNodeData>> paths = GetPaths(childNode, depth + 1);
            result.AddRange(paths);

            if (depth == 0) continue;
            
            foreach (List<MapNodeData> t in paths)
            {
                t.Insert(0, MapNodeData.Create(node));
            }
        }

        return result;
    }
}
