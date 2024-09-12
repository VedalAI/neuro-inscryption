using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuroInscryption.Utilities;

public static class CollectionExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> enumerable) => enumerable.Select((item, index) => (item, index));

    public static T? NullIfEmpty<T>(this T? obj) where T : ICollection => obj != null && obj.Count > 0 ? obj : default;

    public static Dictionary<string, T> ToIndexedDictionary<T>(this IEnumerable<T> enumerable) => enumerable.ToIndexedDictionary(x => x);
    public static Dictionary<string, TValue> ToIndexedDictionary<T, TValue>(this IEnumerable<T> enumerable, Func<T, TValue> valueSelector) => enumerable.WithIndex().ToDictionary(p => p.index.ToString(), p => valueSelector(p.item));
}
