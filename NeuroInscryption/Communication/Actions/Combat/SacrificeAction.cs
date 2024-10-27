using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Utilities;

using Sacrifice = (string name, int blood);

namespace NeuroInscryption.Communication.Actions.Combat;

public sealed class SacrificeAction(PlayableCard cardToPlay, IReadOnlyDictionary<string, PlayableCardData?> playerLanes) : IAction<IEnumerable<PlayableCardData>>
{
    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOfAdd(new Dictionary<string, JsonSchema>
        {
            ["action"] = QJS.Const(ActionName),
            ["data"] = QJS.Enum(GetSacrificeCombinations(cardToPlay.Info.BloodCost))
        });
    }

    private IEnumerable<List<string>> GetSacrificeCombinations(int bloodCost)
    {
        List<Sacrifice> sacrificableSlots = BoardManager.Instance.playerSlots
            .Where(p => p.Card && p.Card.CanBeSacrificed)
            .Select(p => (playerLanes.Values.First(c => c?.TargetCard == p.Card)!.Identifier, p.Card.HasAbility(Ability.TripleBlood) ? 3 : 1))
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

    public string ActionName => ActionStrings.ChooseSacrifices;

    bool IAction<IEnumerable<PlayableCardData>>.Deserialize(Action action, out IEnumerable<PlayableCardData>? response)
    {
        IEnumerable<string?>? cardIdentifiers = action.ActionData?.Values<string>()!;
        if (cardIdentifiers == null)
        {
            response = null;
            return false;
        }

        response = cardIdentifiers.Select(id => playerLanes.Values.First(c => c?.Identifier == id)!);
        return true;
    }
}
