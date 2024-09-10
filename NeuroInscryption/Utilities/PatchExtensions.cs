using HarmonyLib;

namespace NeuroInscryption.Utilities;

internal static class PatchExtensions
{
    public static ref int GetEnumeratorState(this object instance)
    {
        return ref AccessTools.FieldRefAccess<object, int>(instance, AccessTools.Field(instance.GetType(), "<>1__state"));
    }

    public static ref object GetEnumeratorCurrent(this object instance)
    {
        return ref AccessTools.FieldRefAccess<object, object>(instance, AccessTools.Field(instance.GetType(), "<>2__current"));
    }

    public static T GetEnumeratorThis<T>(this object instance)
    {
        return (T) AccessTools.Field(instance.GetType(), "<>4__this").GetValue(instance);
    }
}
