using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace NeuroInscryption.Utilities;

public static class JsonExtensions
{
    public static bool TryValue<T>(this JToken? token, out T? value, object? key = null)
    {
        if (token is null)
        {
            value = default;
            return false;
        }

        try
        {
            value = key is null ? token.Value<T>() : token.Value<T>(key);
            return true;
        }
        catch (InvalidCastException)
        {
            value = default;
            return false;
        }
    }

    public static bool TryValues<T>(this JToken? token, out IEnumerable<T?>? values, object? key = null)
    {
        if (token is null)
        {
            values = null;
            return false;
        }

        try
        {
            values = key is null ? token.Values<T>() : token.Values<T>(key);
            return true;
        }
        catch (InvalidCastException)
        {
            values = null;
            return false;
        }
    }
}
