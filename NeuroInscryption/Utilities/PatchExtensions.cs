using HarmonyLib;

namespace NeuroInscryption.Utilities;

internal static class PatchExtensions
{
    public static ref int GetStateField(this object instance)
    {
        return ref AccessTools.FieldRefAccess<object, int>(instance, AccessTools.Field(instance.GetType(), "<>1__state"));
    }

    public static ref object GetCurrentField(this object instance)
    {
        return ref AccessTools.FieldRefAccess<object, object>(instance, AccessTools.Field(instance.GetType(), "<>2__current"));
    }

    public static ref T GetThisField<T>(this object instance)
    {
        return ref AccessTools.FieldRefAccess<object, T>(instance, AccessTools.Field(instance.GetType(), "<>4__this"));
    }
}
