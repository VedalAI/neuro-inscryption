using System.Diagnostics.CodeAnalysis;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class TotemData
{
    [JsonProperty("Sigil")]
    public string SigilName { get; private set; }
    public string Tribe { get; private set; }

    // Excluded from JSON

    [JsonIgnore]
    public Ability Sigil { get; private set; }

    private TotemData(Totem totem)
    {
        TotemItemData totemData = totem.TotemItemData;
        Sigil = totemData.bottom.effectParams.ability;
        SigilName = SigilData.Create(Sigil).Name;
        Tribe = totemData.top.prerequisites.tribe.ToString();
    }

    [return: NotNullIfNotNull(nameof(totem))]
    public static TotemData? Create(Totem? totem) => totem != null ? new TotemData(totem) : null;
}
