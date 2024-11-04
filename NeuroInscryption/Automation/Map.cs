using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Automation;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
internal sealed class Map
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    private sealed class MapContext(List<List<MapNodeData>> paths) : Common.ContextBase
    {
        public readonly IReadOnlyList<string> Paths = paths.ConvertAll(path => string.Join(" -> ", path.Select(node => node.Identifier)));
        public readonly IDictionary<string, string> Descriptions = paths.Flatten().ToDescriptionDictionary();
    }
    public async UniTask<MapNodeData> ChooseNextNode(CancellationToken ct)
    {
        MapNodeManager instance = MapNodeManager.Instance;

        List<MapNode> directNext = instance.ActiveNode.connectedNodes;
        if (directNext.Count == 1)
        {
            Plugin.Logger.LogDebug($"Only one path ({directNext[0]}), not gonna bother asking");
            return MapNodeData.Create(directNext[0]);
        }

        List<List<MapNodeData>> paths = GetPaths(instance.ActiveNode);
        List<MapNodeData> nextNodes = paths.Select(p => p[0]).Distinct().ToList();

        MapNodeData nextNode = await Neuro.Loadout.ChooseItem(nextNodes, DecisionStrings.ChoosePath, ct, new MapContext(paths));

        return nextNode;
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
