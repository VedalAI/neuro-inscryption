using JetBrains.Annotations;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class BoonData
{
    public string Name { get; }
    public string Description { get; }

    private BoonData(DiskCardGame.BoonData boon)
    {
        Name = boon.displayedName;
        Description = boon.description.ParseDescription();
    }

    public static BoonData Create(DiskCardGame.BoonData boon) => new(boon);
}
