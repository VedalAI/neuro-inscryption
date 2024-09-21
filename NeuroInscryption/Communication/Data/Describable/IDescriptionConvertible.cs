using System.Collections.Generic;
using System.Linq;

namespace NeuroInscryption.Communication.Data.Describable;

public interface IDescriptionConvertible
{
    string Name { get; }
    string Info { get; }
}

public static class DescriptionConvertibleExtensions
{
    public static Description ToDescription(this IDescriptionConvertible descriptionConvertible) => new(descriptionConvertible.Name, descriptionConvertible.Info);

    public static Dictionary<string, string> ToDescriptionDictionary(this IEnumerable<IDescriptionConvertible> describableConvertibles) => describableConvertibles.Distinct().ToDictionary(d => d.Name, d => d.Info);
}
