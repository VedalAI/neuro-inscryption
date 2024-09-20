using System;
using System.Linq;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data;

namespace NeuroInscryption.Communication.Strings;

public static class MapStrings
{
    [Describes(typeof(CardBattleNodeData))]
    private static readonly Describable _cardBattle = ("Card Battle", "A standard card battle");

    [Describes(typeof(TotemBattleNodeData))]
    private static readonly Describable _totemBattle = ("Totem Card Battle", "A more difficult card battle where the opponent has a totem of their own, which grants one of their creature tribes an additional sigil");

    [Describes(typeof(BossBattleNodeData))]
    [Describes(typeof(CabinNodeData))] // probably not in kaycee's mod but who knows
    private static readonly Describable _bossBattle = ("Boss Battle", "A very difficult card battle against a boss");

    [DescribesCardChoice(CardChoicesType.Random)]
    private static readonly Describable _cardChoiceRandom = ("Random Card Choice", "Choose a card from a random selection of 3 cards to add to your deck");

    [DescribesCardChoice(CardChoicesType.Cost)]
    private static readonly Describable _cardChoiceCost = ("Random Card Choice (Cost)", "Choose between three different costs in blood or bones and obtain a random card of that cost to add to your deck");

    [DescribesCardChoice(CardChoicesType.Tribe)]
    private static readonly Describable _cardChoiceTribe = ("Random Card Choice (Tribe)", "Choose between three different tribes and obtain a random card of that tribe to add to your deck");

    [Describes(typeof(CardStatBoostNodeData))]
    private static readonly Describable _campfire = ("Campfire", "Choose a card to either upgrade its health by 2 or attack by 1 (chosen randomly). You can upgrade the card twice, but the second time will have a 50% risk of failing and removing the card from your deck (unless you have already fed the survivors a poisonous card)");

    [Describes(typeof(GainConsumablesNodeData))]
    private static readonly Describable _pack = ("Gain Consumables", "Choose between three random items until you have reached a maximum of 3. If you already have 3 items, you will receive a Pack Rat card [2 power, 2 attack, cost of 2 blood, Trinket Bearer (gives you an item when played)] instead.");

    [Describes(typeof(BuildTotemNodeData))]
    private static readonly Describable _buildTotem = ("The Woodcarver", "Obtain a random totem piece and optionally rebuild your totem. If you already have 7 totem pieces, you will receive an Amalgam card [3 power, 3 attack, no cost, belongs to all tribes] instead.");

    [Describes(typeof(BuyPeltsNodeData))]
    private static readonly Describable _buyPelts = ("The Trapper", "Purchase pelts or a Skinning Knife [an item that allows you to destroy one card from the opponent] using the teeth you have accumulated");

    [Describes(typeof(TradePeltsNodeData))]
    private static readonly Describable _tradePelts = ("The Trader", "Exchange pelts in your deck for new cards");

    [Describes(typeof(CardRemoveNodeData))]
    private static readonly Describable _cardRemove = ("Bone Altar", "Sacrifice a card, removing it from your deck permanently. You will get a Minor Boon of the Bone Lord in exchange"); // TODO: boon descriptions

    [Describes(typeof(DuplicateMergeNodeData))]
    private static readonly Describable _duplicateMerge = ("The Mycologists", "Combine two versions of the same card together to create a stronger version with stats equal to the combined stats of the original cards. Sigils will be combined as well, but duplicate sigils will only be counted once");

    [Describes(typeof(BoulderChoiceNodeData))]
    private static readonly Describable _boulderChoice = ("The Prospector", "Get a random card with a 66% chance for an insect card that may have a random additional sigil, and a 33% chance for a Golden Pelt [0 power, 3 health, can be traded for a rare card]");

    [Describes(typeof(CopyCardNodeData))]
    private static readonly Describable _copyCard = ("Goobert", "Choose a card to be duplicated by Goobert. The duplicated copy may have errors such as slightly altered stats or incorrect sigils");

    [Describes(typeof(CardMergeNodeData))]
    private static readonly Describable _cardMerge = ("Mysterious Stones", "Sacrifice a card and transfer its sigils to another card");

    [Describes(typeof(DeckTrialNodeData))]
    private static readonly Describable _deckTrial = ("Deck Trial", "Get a chance at obtaining a random card with 2 additional sigils based on a deck trial of your chooice.");

    // ---

    public static Describable? Get(NodeData nodeData)
    {
        FieldInfo? fieldInfo = AccessTools.GetDeclaredFields(typeof(MapStrings)).SingleOrDefault(f => f.GetCustomAttributes<DescribesAttribute>().Any(a => a.Matches(nodeData)));
        return fieldInfo != null ? (Describable) fieldInfo.GetValue(null) : null;
    }
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
file class DescribesCardChoiceAttribute(CardChoicesType type) : DescribesAttribute(typeof(CardChoicesNodeData))
{
    public override bool Matches(NodeData data) => base.Matches(data) && ((CardChoicesNodeData) data).choicesType == type;
}

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
file class DescribesAttribute(Type nodeDataType) : Attribute
{
    public virtual bool Matches(NodeData data) => nodeDataType == data.GetType();
}
