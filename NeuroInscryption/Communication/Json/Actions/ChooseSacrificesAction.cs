using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Utilities;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Communication.Json.Actions;

using Sacrifice = (string name, int blood);

public sealed class ChooseSacrificesAction(PlayableCard cardToPlay, GameState.Board state) : ISchemaModifier
{
    public void Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const("choose_sacrifices"),
                ["data"] = QJS.Enum(GetSacrificeCombinations(cardToPlay.Info.BloodCost))
            }
        });
    }

    private IEnumerable<List<string>> GetSacrificeCombinations(int bloodCost)
    {
        List<Sacrifice> sacrificableSlots = BoardManager.Instance.playerSlots
            .Where(p => p.Card && p.Card.CanBeSacrificed)
            .Select(p => (state.PlayerLanes.Values.First(c => c?.TargetCard == p.Card)!.Name, p.Card.HasAbility(Ability.TripleBlood) ? 3 : 1))
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
}
