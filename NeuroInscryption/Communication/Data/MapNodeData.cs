using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;

namespace NeuroInscryption.Communication.Data;

public sealed class MapNodeData : IComparable<MapNodeData>, IEquatable<MapNodeData>
{
    public readonly string Name;
    public readonly string Description;

    private MapNodeData(NodeData nodeData)
    {
        Describable? describable = MapStrings.Get(nodeData);
        if (describable == null) throw new ArgumentException($"Node data {nodeData} not found in strings");

        Name = describable.Name;
        Description = describable.Description;
    }

    public static MapNodeData Create(NodeData nodeData) => new(nodeData);

    public static implicit operator MapNodeData(NodeData nodeData) => Create(nodeData);

    #region Equality

    public int CompareTo(MapNodeData? other) => string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    public bool Equals(MapNodeData? other) => Name == other?.Name && Description == other.Description;
    public override bool Equals(object? obj) => obj is MapNodeData other && Equals(other);
    public override int GetHashCode() => unchecked((Name.GetHashCode() * 397) ^ Description.GetHashCode());
    public static bool operator ==(MapNodeData? left, MapNodeData? right) => Equals(left, right);
    public static bool operator !=(MapNodeData? left, MapNodeData? right) => !(left == right);

    #endregion
}
