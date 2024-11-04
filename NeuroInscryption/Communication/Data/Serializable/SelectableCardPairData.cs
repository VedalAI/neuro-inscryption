using System.Collections.Generic;
using DiskCardGame;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class SelectableCardPairData : IIdentifiable
{
    public string Identifier { get; private set; }

    public IReadOnlyList<CardData> Cards { get; private set; }

    // Ignored from JSON

    [JsonIgnore]
    public SelectableCardPair Pair { get; private set; }

    private SelectableCardPairData(SelectableCardPair pair)
    {
        Pair = pair;
        Cards =
        [
            CardData.Create(pair.leftCard.Info),
            CardData.Create(pair.rightCard.Info)
        ];
        Identifier = pair.leftCard.Info.DisplayedNameEnglish;
    }

    public static SelectableCardPairData Create(SelectableCardPair pair) => new(pair);
}
