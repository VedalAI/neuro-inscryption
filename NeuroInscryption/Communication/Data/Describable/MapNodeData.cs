using System;
using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data.Describable;

public sealed class MapNodeData : IComparable<MapNodeData>, IEquatable<MapNodeData>, IDescribable, IIdentifiable
{
    public string Identifier { get; }
    public string Name { get; }
    public string Info { get; }
    public MapNode MapNode { get; }

    private MapNodeData(MapNode node)
    {
        MapNode = node;

        Description? describable = MapStrings.Get(node.Data);
        if (describable == null) throw new ArgumentException($"Node data {node.Data} not found in strings");

        Name = describable.Name;
        Info = describable.Info;

        List<MapNode> connectedNodes = MapNodeManager.Instance.GetNodeWithId(RunState.Run.currentNodeId).connectedNodes;
        if (connectedNodes.Contains(node))
        {
            Identifier = Name + string.Format(ActionStrings.PathDiscriminator, connectedNodes.IndexOf(node));
        }
        else
        {
            Identifier = Name;
        }
    }

    public static MapNodeData Create(MapNode node) => new(node);

    #region Equality

    public int CompareTo(MapNodeData? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    public bool Equals(MapNodeData? other) => Name == other?.Name && Info == other.Info;
    public override bool Equals(object? obj) => obj is MapNodeData other && Equals(other);
    public override int GetHashCode() => unchecked((Name.GetHashCode() * 397) ^ Info.GetHashCode());
    public static bool operator ==(MapNodeData? left, MapNodeData? right) => Equals(left, right);
    public static bool operator !=(MapNodeData? left, MapNodeData? right) => !(left == right);

    #endregion
}
