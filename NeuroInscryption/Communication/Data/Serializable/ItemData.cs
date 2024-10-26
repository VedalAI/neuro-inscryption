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

    public bool? IsUsable => // CanBeUsedRightNow
        !ConsumableItem.ConsumablesCanBeActivated() ? null // omit from json
            : Item.ExtraActivationPrerequisitesMet();

    [JsonIgnore]
    public ItemSlot? Slot { get; init; }
    [JsonIgnore]
    public ConsumableItem Item { get; init; }
    [JsonIgnore]
    public ConsumableItemData Data => (ConsumableItemData)Item.Data;

    private ItemData(ConsumableItem item)
    {
        Item = item;

        Identifier = Data.rulebookName;
        Description = Data.rulebookDescription.ParseDescription();
    }

    public static ItemData Create(ItemSlot slot) => new((ConsumableItem)slot.Item) { Slot = slot };
    public static ItemData Create(ConsumableItem item) => new(item);
}