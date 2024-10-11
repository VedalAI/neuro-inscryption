using System;
using HarmonyLib;

namespace NeuroInscryption.Utilities;

public static class PatchExtensions
{
    public static T GetEnumeratorField<T>(this object instance, string fieldName)
    {
        try
        {
            return (T) AccessTools.Field(instance.GetType(), fieldName).GetValue(instance);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e);
            return default!;
        }
    }

    public static int GetEnumeratorState(this object instance) => instance.GetEnumeratorField<int>("<>1__state");
    public static object GetEnumeratorCurrent(this object instance) => instance.GetEnumeratorField<object>("<>2__current");
    public static T GetEnumeratorThis<T>(this object instance) => instance.GetEnumeratorField<T>("<>4__this");
}
