namespace NeuroInscryption.Communication.Types;

internal abstract class StrObject
{
    protected abstract string GetString();

    public sealed override string ToString() => GetString();
}
