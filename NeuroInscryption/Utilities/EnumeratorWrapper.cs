using System.Collections;

namespace NeuroInscryption.Utilities;

public readonly struct EnumeratorWrapper<TThis>(IEnumerator enumerator)
{
    public IEnumerator Enumerator { get; init; } = enumerator;

    public int State
    {
        get => Enumerator.GetState();
        set => Enumerator.SetEnumeratorState(value);
    }

    public object? Current
    {
        get => Enumerator.GetCurrent();
        set => Enumerator.SetEnumeratorCurrent(value!);
    }

    public TThis This
    {
        get => Enumerator.GetThis<TThis>();
        set => Enumerator.SetEnumeratorThis(value);
    }

    public T GetFieldValue<T>(string fieldName)
        => Enumerator.GetFieldValue<T>(fieldName);
    public void SetFieldValue<T>(string fieldName, T value)
        => Enumerator.SetFieldValue(fieldName, value);
}
