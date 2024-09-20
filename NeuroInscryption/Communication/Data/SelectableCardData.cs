using System;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.State;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class SelectableCardData
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

        CardChoice info = card.ChoiceInfo;
        if (info.CardInfo != null)
        {
            Card = CardData.Create(info.CardInfo, state);
            Identifier = Card.DisplayName + string.Format(CardStrings.GenericDiscriminator, index);
        }
        else if (info.tribe != DiskCardGame.Tribe.None)
        {
            Identifier = Tribe = info.tribe.ToString();
        }
        else
        {
            switch (info.resourceType)
            {
                case ResourceType.Blood:
                    Identifier = Cost = string.Format(CardStrings.BloodCost, info.resourceAmount);
                    break;
                case ResourceType.Bone:
                    Identifier = Cost = PluralFormatProvider.Format(CardStrings.BonesCost, info.resourceAmount);
                    break;

                case ResourceType.None:
                    throw new InvalidOperationException("Cannot create SelectableCardData for uninitialized CardChoice");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static SelectableCardData Create(SelectableCard card, int index, IGameState state) => new(card, index, state);
}
