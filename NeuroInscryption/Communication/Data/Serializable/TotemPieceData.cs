using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class TotemPieceData : IIdentifiable
{
    [JsonProperty("Sigil")]
    public string? SigilName { get; private set; }
    public string? Tribe { get; private set; }

    // Excluded from JSON

    [JsonIgnore]
    public Ability? Sigil { get; private set; }

    [JsonIgnore]
    public string Identifier { get; private set; }

    [JsonIgnore]
    public SelectableItemSlot? Slot { get; private set; }

    private TotemPieceData(Tribe tribe, SelectableItemSlot? slot = null)
    {
        Identifier = Tribe = tribe.ToString();
        Slot = slot;
    }

    private TotemPieceData(Ability sigil, SelectableItemSlot? slot = null)
    {
        Sigil = sigil;
        Identifier = SigilName = SigilData.Create(sigil).Name;
        Slot = slot;
    }

    public static TotemPieceData Create(Tribe tribe) => new(tribe);
    public static TotemPieceData Create(Ability sigil) => new(sigil);
    public static TotemPieceData? Create(SelectableItemSlot slot) => slot.Item.Data switch
    {
        TotemTopData topData => new TotemPieceData(topData.prerequisites.tribe, slot),
        TotemBottomData bottomData => new TotemPieceData(bottomData.effectParams.ability, slot),
        _ => null
    };
}
