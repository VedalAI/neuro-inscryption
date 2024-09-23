using System.Collections.Generic;
using DiskCardGame;
using JetBrains.Annotations;
using NeuroInscryption.Communication.Data.State;
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

    private SelectableCardPairData(SelectableCardPair pair, IGameState state)
    {
        Pair = pair;
        Cards =
        [
            CardData.Create(pair.leftCard.Info, state),
            CardData.Create(pair.rightCard.Info, state)
        ];
        Identifier = pair.leftCard.Info.DisplayedNameEnglish;
    }

    public static SelectableCardPairData Create(SelectableCardPair pair, IGameState state) => new(pair, state);
}
