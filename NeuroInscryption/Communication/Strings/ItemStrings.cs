using System.Collections.Generic;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Describable;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Strings;

public static class ItemStrings
{
    private static readonly Dictionary<string, string> _descriptionOverrides = new()
    {
        ["TrapperKnife"] = "Target a cuttable opponent card. That card is destroyed and a Pelt is created in your hand.",
        ["FishHook"] = "Target a cuttable opponent card that is not opposed by any of your cards. That card will be pulled to your side.",
        ["BleachPot"] = "All opponent cards lose their sigils. Cannot be used if none of the opponent cards have sigils.",
        ["BirdLegFan"] = "All your creatures currently in play gain the Airborne sigil until the end of your turn.",
        ["Hourglass"] = "Opponent's next turn is skipped.",
        ["MagnifyingGlass"] = "Pick any card from your deck to draw into your hand.",
        ["PiggyBank"] = "Immediately gain 4 bones.",
        ["Pliers"] = "Use one of your own teeth to add weight to the scale (equivalent to 1 damage).",
        // todo: lane number mentioned
        ["PocketWatch"] = "All cards in play move one space clockwise. This means your cards move left and the opponent's cards move right - with the exception of lane 0, where your card moves to the opponent's side, and lane 3, where the opponent's card moves to your side.",
        ["Scissors"] = "Target a cuttable opponent card and destroy it.",
    };

    public static Description Get(ConsumableItem item)
    {
        ConsumableItemData data = (ConsumableItemData)item.Data;
        string info = _descriptionOverrides.TryGetValue(data.name, out string overrideDesc)
            ? overrideDesc
            : data.rulebookDescription.ParseDescription();
        return new(data.rulebookName, info);
    }
}
