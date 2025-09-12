using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;
using NeuroSdk.Json;
using Newtonsoft.Json.Linq;
using Sacrifice = (string name, int blood);

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class SacrificeAction(PlayableCard cardToPlay, IReadOnlyDictionary<string, PlayableCardData?> playerLanes)
    : ActionBase<IEnumerable<PlayableCardData>>(ActionStrings.ChooseSacrifices)
{
    private IEnumerable<List<string>> GetSacrificeCombinations(int bloodCost)
    {
        List<Sacrifice> sacrificableSlots = BoardManager.Instance.playerSlots
            .Where(p => p.Card && p.Card.CanBeSacrificed)
            .Select(p => (playerLanes.Values.First(c => c?.TargetCard == p.Card)!.Identifier, GetBloodYield(p.Card)))
            .ToList();

        List<List<Sacrifice>> possibilities = DontOpenJumpscareWarning.GenerateAllPermutationsOfSubsets(sacrificableSlots);
        for (int i = 0; i < possibilities.Count; i++)
        {
            List<Sacrifice> possibility = possibilities[i];
            int beforeLast = possibility.Take(possibility.Count - 1).Sum(p => p.blood);
            int totalBlood = possibility.Sum(p => p.blood);

            bool ok = beforeLast < bloodCost && bloodCost <= totalBlood;
            if (!ok)
            {
                possibilities.RemoveAt(i);
                i--;
            }
        }

        return possibilities.Select(p => p.Select(s => s.name).ToList());
    }

    public override JsonSchema GetDataSchema()
    {
        return new JsonSchema
        {
            Type = JsonSchemaType.Object,
            Properties = new Dictionary<string, JsonSchema>
            {
                ["sacrifices"] = QJS.Enum(GetSacrificeCombinations(cardToPlay.Info.BloodCost).Select(combo => string.Join(", ", combo)))
            },
            Required = ["sacrifices"]
        };
    }

    protected override bool TryDeserialize(JToken? data, [NotNullWhen(true)] out IEnumerable<PlayableCardData>? response)
    {
        IEnumerable<string?>? cardIdentifiers = data?.Value<string>("sacrifices")?.Split([", "], StringSplitOptions.None);
        if (cardIdentifiers == null)
        {
            response = null;
            return false;
        }
        
        List<string?> cardIdentifiersList = cardIdentifiers.ToList();
        HashSet<string?> uniqueIdentifiers = new(cardIdentifiersList);
        if (uniqueIdentifiers.Count != cardIdentifiersList.Count || uniqueIdentifiers.Any(i => i == null))
        {
            response = null;
            return false;
        }

        response = cardIdentifiers.Select(id => playerLanes.Values.FirstOrDefault(c => c?.Identifier == id)!).ToArray();
        return response.All(c => c is { CanBeSacrificed: true }) && response.Sum(card => GetBloodYield(card.TargetCard)) >= cardToPlay.Info.BloodCost;
    }
    
    private static int GetBloodYield(PlayableCard card) => card.HasAbility(Ability.TripleBlood) ? 3 : 1;
}
