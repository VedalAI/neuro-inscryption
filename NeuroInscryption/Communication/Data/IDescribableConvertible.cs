using System.Collections.Generic;
using System.Linq;

namespace NeuroInscryption.Communication.Data;

public interface IDescribableConvertible
{
    string Name { get; }
    string Description { get; }
}

// ReSharper disable once InconsistentNaming
public static class IDescribableConvertibleExtensions
{
    public static Describable ToDescribable(this IDescribableConvertible describableConvertible) => new(describableConvertible.Name, describableConvertible.Description);

    public static Dictionary<string, string> ToDescribableDictionary(this IEnumerable<IDescribableConvertible> describableConvertibles) => describableConvertibles.Distinct().ToDictionary(d => d.Name, d => d.Description);
}
