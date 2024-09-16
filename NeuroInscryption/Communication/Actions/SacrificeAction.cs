using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Messages.Incoming;
using NeuroInscryption.Utilities;
using GameState = NeuroInscryption.Communication.Data.GameState;

namespace NeuroInscryption.Communication.Actions;

using Sacrifice = (string name, int blood);

public sealed class SacrificeAction(PlayableCard cardToPlay, GameState.Board state) : IAction<IEnumerable<string>>
{
    private const string _actionName = "choose_sacrifices";

    void ISchemaModifier.Apply(JsonSchema schema)
    {
        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const(_actionName),
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

    string IAction.ActionName => _actionName;
    bool IAction<IEnumerable<string>>.Deserialize(Action action, out IEnumerable<string>? response)
    {
        response = action.ActionData?.Values<string>()!;
        return response != null;
    }
}
