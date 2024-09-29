using DiskCardGame;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class ItemData : IIdentifiable
{
    [JsonProperty("Name")]
    public string Identifier { get; }
    public string Description { get; }

    // Ignored from json

    [JsonIgnore]
    public ItemSlot? Slot { get; }

    private ItemData(ItemSlot slot)
    {
        ConsumableItemData itemData = (ConsumableItemData) slot.Item.Data;

        Identifier = itemData.rulebookName;
        Description = itemData.rulebookDescription.ParseDescription();
        Slot = slot;
    }

    public static ItemData Create(ItemSlot slot) => new(slot);
}
