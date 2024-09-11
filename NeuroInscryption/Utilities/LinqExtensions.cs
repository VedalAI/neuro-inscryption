using System.Collections.Generic;
using System.Linq;

namespace NeuroInscryption.Utilities;

internal static class LinqExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable) => enumerable.Select((item, index) => (item, index));
}
