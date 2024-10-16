using System;
using System.Collections;

namespace NeuroInscryption.Utilities;

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
            return default!;
        }
    }

    public static int GetState(this IEnumerator instance) => instance.GetFieldValue<int>(StateFieldName);
    public static object GetCurrent(this IEnumerator instance) => instance.GetFieldValue<object>(CurrentFieldName);
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
        }
    }
    public static void SetEnumeratorState(this IEnumerator instance, int value) => instance.SetFieldValue(StateFieldName, value);
    public static void SetEnumeratorCurrent(this IEnumerator instance, object value) => instance.SetFieldValue(CurrentFieldName, value);
    public static void SetEnumeratorThis<T>(this IEnumerator instance, T value) => instance.SetFieldValue(ThisFieldName, value);

    public static EnumeratorWrapper<object> Wrap(this IEnumerator enumerator) => new(enumerator);
    public static EnumeratorWrapper<TThis> Wrap<TThis>(this IEnumerator enumerator) => new(enumerator);
}
