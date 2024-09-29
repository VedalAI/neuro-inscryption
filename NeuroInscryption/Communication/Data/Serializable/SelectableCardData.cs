using System;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class SelectableCardData : IIdentifiable
{
    public string? Cost { get; private set; }
    public string? Tribe { get; private set; }
    public CardData? Card { get; private set; }

    // Excluded from JSON

    [JsonIgnore]
    public string Identifier { get; private set; }

    [JsonIgnore]
    public SelectableCard TargetCard { get; private set; }

    private SelectableCardData(SelectableCard card, int index, IGameState state)
    {
        TargetCard = card;

        if (card.Info != null)
        {
            Card = CardData.Create(card.Info, state);
            Identifier = Card.DisplayName;
            if (index != -1) Identifier += string.Format(CardStrings.GenericDiscriminator, index);
        }
        else if (card.ChoiceInfo.CardInfo != null)
        {
            Card = CardData.Create(card.ChoiceInfo.CardInfo, state);
            Identifier = Card.DisplayName;
            if (index != -1) Identifier += string.Format(CardStrings.GenericDiscriminator, index);
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
                    Identifier = Cost = string.Format(CardStrings.BloodCost, card.ChoiceInfo.resourceAmount);
                    break;
                case ResourceType.Bone:
                    Identifier = Cost = PluralFormatProvider.Format(CardStrings.BonesCost, card.ChoiceInfo.resourceAmount);
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
