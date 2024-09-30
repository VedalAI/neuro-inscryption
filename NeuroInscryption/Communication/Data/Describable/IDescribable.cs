using System.Collections.Generic;
using System.Linq;

namespace NeuroInscryption.Communication.Data.Describable;

public interface IDescribable
{
    string Name { get; }
    string Info { get; }
}

public static class DescribableExtensions
{
    public static Description ToDescription(this IDescribable describable) => new(describable.Name, describable.Info);

    public static Dictionary<string, string> ToDescriptionDictionary(this IEnumerable<IDescribable> describableConvertibles) => describableConvertibles.Distinct().ToDictionary(d => d.Name, d => d.Info);
}
