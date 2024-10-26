using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Communication.Strings;
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
        Description desc = ItemStrings.Get(item);
        Identifier = desc.Name;
        Description = desc.Info;
    }

    public static ItemData Create(ItemSlot slot) => new((ConsumableItem)slot.Item) { Slot = slot };
    public static ItemData Create(ConsumableItem item) => new(item);
}
