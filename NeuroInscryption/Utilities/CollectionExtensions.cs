using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuroInscryption.Utilities;

public static class CollectionExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable) => enumerable.Select((item, index) => (item, index));

    public static T? NullIfEmpty<T>(this T? obj) where T : ICollection => obj != null && obj.Count > 0 ? obj : default;
}
