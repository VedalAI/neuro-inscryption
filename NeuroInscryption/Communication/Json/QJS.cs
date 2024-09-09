using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations;

namespace NeuroInscryption.Communication.Json;

// ReSharper disable once InconsistentNaming
public static class QJS
{
    public static JsonSchema EmptyArray => new()
    {
        Type = JsonSchemaType.Array,
        MaxItems = 0
    };

    public static JsonSchema Const<T>(T value)
    {
        return new JsonSchema
        {
            Const = value
        };
    }

    public static JsonSchema Array<T>(IList<T> items)
    {
        return new JsonSchema
        {
            Type = JsonSchemaType.Array,
            MinItems = items.Count,
            MaxItems = items.Count,
            PrefixItems = items.Select(Const).ToList()
        };
    }

    public static JsonSchema Enum<T>(IEnumerable<T> values)
    {
        return new JsonSchema
        {
            Enum = values.Cast<object>().ToList()
        };
    }
}
