using System;
using DiskCardGame;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroInscryption.Utilities.Strings;
using Newtonsoft.Json;

namespace NeuroInscryption.Communication.Data.Serializable;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class DeckTrialData : IIdentifiable
{
    [JsonProperty("Name")]
    public string Identifier { get; }

    public string Description { get; }

    // Ignored from JSON

    [JsonIgnore]
    public SelectableCard Card { get; }

    private DeckTrialData(DeckTrialSequencer.Trial trial, SelectableCard card)
    {
        Identifier = trial.type switch
        {
            DeckTrialSequencer.Trial.Type.Wisdom => StringFormatter.Format(TrialStrings.Sigils, trial.threshold),
            DeckTrialSequencer.Trial.Type.Blood => StringFormatter.Format(TrialStrings.Blood, trial.threshold),
            DeckTrialSequencer.Trial.Type.Bones => StringFormatter.Format(TrialStrings.Bones, trial.threshold),
            DeckTrialSequencer.Trial.Type.Power => StringFormatter.Format(TrialStrings.Power, trial.threshold),
            DeckTrialSequencer.Trial.Type.Health => StringFormatter.Format(TrialStrings.Health, trial.threshold),
            DeckTrialSequencer.Trial.Type.Kin => TrialStrings.Kin,
            _ => throw new ArgumentOutOfRangeException()
        };
        Description = trial.description.ParseDescription();
        Card = card;
    }

    public static DeckTrialData Create(DeckTrialSequencer.Trial trial, SelectableCard card) => new(trial, card);
}
