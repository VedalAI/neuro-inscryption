using DiskCardGame;
using JetBrains.Annotations;
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
    public SelectableItemSlot? Slot { get; }

    private ItemData(Item item)
    {
        ConsumableItemData itemData = (ConsumableItemData) item.Data;

        Identifier = itemData.rulebookName;
        Description = itemData.rulebookDescription;
    }

    private ItemData(SelectableItemSlot slot) : this(slot.Item)
    {
        Slot = slot;
    }

    public static ItemData Create(Item item) => new(item);
    public static ItemData Create(SelectableItemSlot slot) => new(slot);
}
