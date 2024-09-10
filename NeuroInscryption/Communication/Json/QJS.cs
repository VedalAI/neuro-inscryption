using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations;

namespace NeuroInscryption.Communication.Json;

// ReSharper disable once InconsistentNaming
public static class QJS
{
    public static JsonSchema ConstEmptyArray => Const(Array.Empty<object>());

    public static JsonSchema Const<T>(T value)
    {
        return new JsonSchema
        {
            Const = value
        };
    }

    public static JsonSchema Enum<T>(IEnumerable<T> values)
    {
        return new JsonSchema
        {
            Enum = values.Cast<object>().ToList()
        };
    }

    public static JsonSchema Type(JsonSchemaType type)
    {
        return new JsonSchema
        {
            Type = type
        };
    }
}
