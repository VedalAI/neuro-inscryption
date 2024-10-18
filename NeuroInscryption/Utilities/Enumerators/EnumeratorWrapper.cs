using System.Collections;

namespace NeuroInscryption.Utilities.Enumerators;

public readonly struct EnumeratorWrapper<TThis>(IEnumerator enumerator)
{
    public TThis This => enumerator.GetThis<TThis>();

    public int State
    {
        get => enumerator.GetState();
        set => enumerator.SetState(value);
    }

    public object? Current
    {
        get => enumerator.GetCurrent();
        set => enumerator.SetCurrent(value!);
    }

    public T GetFieldValue<T>(string fieldName)
        => enumerator.GetFieldValue<T>(fieldName);
    public void SetFieldValue<T>(string fieldName, T value)
        => enumerator.SetFieldValue(fieldName, value);
}
