namespace NeuroInscryption.Communication.Types;

internal sealed class StrCard : StrObject
{
    public string Name { get; }

    private StrCard(string name)
    {
        Name = name;
    }

    protected override string GetString()
    {
        return Name;
    }

    public static readonly StrCard Amalgam = new("Amalgam");
}
