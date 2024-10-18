using System;
using System.Collections;
using NeuroInscryption.Utilities.Reflection;
using UnityEngine;

namespace NeuroInscryption.Utilities.Enumerators;

public static class PatchExtensions
{
    private const string StateFieldName = "<>1__state";
    private const string CurrentFieldName = "<>2__current";
    private const string ThisFieldName = "<>4__this";

    public static T GetFieldValue<T>(this object instance, string fieldName)
    {
        try
        {
            return (T) ReflectionCache.GetField(instance.GetType(), fieldName).GetValue(instance);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
            Plugin.Logger.LogDebug(StackTraceUtility.ExtractStackTrace());
            return default!;
        }
    }

    public static int GetState(this IEnumerator instance) => instance.GetFieldValue<int>(StateFieldName);
    public static object? GetCurrent(this IEnumerator instance) => instance.GetFieldValue<object?>(CurrentFieldName);
    public static T GetThis<T>(this IEnumerator instance) => instance.GetFieldValue<T>(ThisFieldName);

    public static void SetFieldValue<T>(this object instance, string fieldName, T value)
    {
        try
        {
            ReflectionCache.GetField(instance.GetType(), fieldName).SetValue(instance, value);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
            Plugin.Logger.LogDebug(StackTraceUtility.ExtractStackTrace());
        }
    }
    public static void SetState(this IEnumerator instance, int value) => instance.SetFieldValue(StateFieldName, value);
    public static void SetCurrent(this IEnumerator instance, object value) => instance.SetFieldValue(CurrentFieldName, value);

    public static EnumeratorWrapper<object> Wrap(this IEnumerator enumerator) => new(enumerator);
    public static EnumeratorWrapper<TThis> Wrap<TThis>(this IEnumerator enumerator) => new(enumerator);
}
