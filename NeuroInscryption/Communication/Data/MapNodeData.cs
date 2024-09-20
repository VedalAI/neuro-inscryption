using System;
using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data;

public sealed class MapNodeData : IComparable<MapNodeData>, IEquatable<MapNodeData>, IDescribableConvertible
{
    public string Name { get; }
    public string NameWithDiscriminator { get; }
    public string Description { get; }
    public MapNode MapNode { get; }

    private MapNodeData(MapNode node)
    {
        MapNode = node;

        Describable? describable = MapStrings.Get(node.Data);
        if (describable == null) throw new ArgumentException($"Node data {node.Data} not found in strings");

        Name = describable.Name;
        Description = describable.Description;

        List<MapNode> connectedNodes = MapNodeManager.Instance.GetNodeWithId(RunState.Run.currentNodeId).connectedNodes;
        if (connectedNodes.Contains(node))
        {
            NameWithDiscriminator = Name + string.Format(ActionStrings.PathDiscriminator, connectedNodes.IndexOf(node));
        }
        else
        {
            NameWithDiscriminator = Name;
        }
    }

    public static MapNodeData Create(MapNode node) => new(node);

    public static implicit operator MapNodeData(MapNode node) => Create(node);

    #region Equality

    public int CompareTo(MapNodeData? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    public bool Equals(MapNodeData? other) => Name == other?.Name && Description == other.Description;
    public override bool Equals(object? obj) => obj is MapNodeData other && Equals(other);
    public override int GetHashCode() => unchecked((Name.GetHashCode() * 397) ^ Description.GetHashCode());
    public static bool operator ==(MapNodeData? left, MapNodeData? right) => Equals(left, right);
    public static bool operator !=(MapNodeData? left, MapNodeData? right) => !(left == right);

    #endregion
}
