using System;

namespace NeuroInscryption.Utilities;

public class UniLazy<T>(Func<T> initializer)
    where T : UnityEngine.Object
{
    private T? _value;
    public T Value => _value != null ? _value : _value = initializer();
}
