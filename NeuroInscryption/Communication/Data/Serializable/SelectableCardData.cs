using System;
using DiskCardGame;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class SelectableCardData : IIdentifiable
{
    [JsonProperty]
    private string? Cost { get; set; }

    [JsonProperty]
    private string? Tribe { get; set; }

    [JsonProperty("Identifier")]
    private string? CardIdentifier { get; set; }

    public CardData? Card { get; private set; }

    // Excluded from JSON

    [JsonIgnore]
    public string Identifier { get; private set; }

    [JsonIgnore]
    public Card TargetCard { get; private set; }

    private SelectableCardData(Card card, int index = -1, IGameState? state = null)
    {
        TargetCard = card;

        if (card.Info != null)
        {
            Card = CardData.Create(card.Info, state);
            CardIdentifier = Card.DisplayName;
            if (index != -1) CardIdentifier += CardStrings.GenericDiscriminator.Format(index);

            Identifier = CardIdentifier;
            return;
        }

        if (card is not SelectableCard selectable)
            throw new InvalidOperationException("Card must either have Card.Info or be a SelectableCard");

        if (selectable.ChoiceInfo.CardInfo != null)
        {
            Card = CardData.Create(selectable.ChoiceInfo.CardInfo);
            CardIdentifier = Card.DisplayName;
            if (index != -1) CardIdentifier += CardStrings.GenericDiscriminator.Format(index);

            Identifier = CardIdentifier;
        }
        else if (selectable.ChoiceInfo.tribe != DiskCardGame.Tribe.None)
        {
            Identifier = Tribe = selectable.ChoiceInfo.tribe.ToString();
        }
        else
        {
            Identifier = Cost = selectable.ChoiceInfo.resourceType switch
            {
                ResourceType.Blood => CardStrings.BloodCost.Format(selectable.ChoiceInfo.resourceAmount),
                ResourceType.Bone => CardStrings.BonesCost.Format("random amount of"),
                ResourceType.None => throw new InvalidOperationException("Cannot create SelectableCardData for uninitialized CardChoice"),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public static SelectableCardData Create(Card card, int index = -1) => Create(card, index, null);
    public static SelectableCardData Create(Card card, int index, IGameState? state) => new(card, index, state);
}
