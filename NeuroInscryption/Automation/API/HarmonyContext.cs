using System.Reflection;

namespace NeuroInscryption.Automation.API;

public sealed class HarmonyContext(MethodInfo originalMethod, object? instance, object? result, object[] parameters)
{
    public MethodInfo OriginalMethod { get; } = originalMethod;
    public object? GetInstance() => instance;
    public T? GetInstance<T>() => (T?) instance;
    public T? GetResult<T>() => (T?) result;
    public T GetParameter<T>(int index) => (T) parameters[index];
}
