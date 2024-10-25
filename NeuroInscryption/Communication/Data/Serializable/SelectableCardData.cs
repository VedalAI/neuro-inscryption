using System;
using DiskCardGame;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Strings;
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
    public SelectableCard TargetCard { get; private set; }

    private SelectableCardData(SelectableCard card, int index, IGameState state) // todo remove state, only used for ijiraq
    {
        TargetCard = card;

        if (card.Info != null)
        {
            Card = CardData.Create(card.Info);
            CardIdentifier = Card.DisplayName;
            if (index != -1) CardIdentifier += StringFormatter.Format(CardStrings.GenericDiscriminator, index);

            Identifier = CardIdentifier;
        }
        else if (card.ChoiceInfo.CardInfo != null)
        {
            Card = CardData.Create(card.ChoiceInfo.CardInfo);
            CardIdentifier = Card.DisplayName;
            if (index != -1) CardIdentifier += StringFormatter.Format(CardStrings.GenericDiscriminator, index);

            Identifier = CardIdentifier;
        }
        else if (card.ChoiceInfo.tribe != DiskCardGame.Tribe.None)
        {
            Identifier = Tribe = card.ChoiceInfo.tribe.ToString();
        }
        else
        {
            switch (card.ChoiceInfo.resourceType)
            {
                case ResourceType.Blood:
                    Identifier = Cost = StringFormatter.Format(CardStrings.BloodCost, card.ChoiceInfo.resourceAmount);
                    break;
                case ResourceType.Bone:
                    Identifier = Cost = StringFormatter.Format(CardStrings.BonesCost, card.ChoiceInfo.resourceAmount);
                    break;

                case ResourceType.None:
                    throw new InvalidOperationException("Cannot create SelectableCardData for uninitialized CardChoice");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static SelectableCardData Create(SelectableCard card, IGameState state) => new(card, -1, state);
    public static SelectableCardData Create(SelectableCard card, int index, IGameState state) => new(card, index, state);
}
