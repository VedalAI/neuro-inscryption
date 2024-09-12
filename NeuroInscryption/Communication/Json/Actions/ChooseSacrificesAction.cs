using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using NeuroInscryption.Utilities;

namespace NeuroInscryption.Communication.Json.Actions;

public sealed class ChooseSacrificesAction(PlayableCard cardToPlay) : ISchemaModifier
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

    private static List<List<int>> GetSacrificeCombinations(int bloodCost)
    {
        List<(int index, int blood)> sacrificableSlots = BoardManager.Instance.playerSlots
            .WithIndex()
            .Where(p => p.item.Card && p.item.Card.CanBeSacrificed)
            .Select(p => (p.index, p.item.Card.HasAbility(Ability.TripleBlood) ? 3 : 1))
            .ToList();

        List<List<int>> combinations = [];
        List<int> currentCombination = [];
        int currentBlood = 0;

        backtracking(0);
        return combinations;

        void backtracking(int index)
        {
            if (currentBlood >= bloodCost)
            {
                combinations.Add([..currentCombination]);
                return;
            }

            for (int i = index; i < sacrificableSlots.Count; i++)
            {
                currentCombination.Add(sacrificableSlots[i].index);
                currentBlood += sacrificableSlots[i].blood;
                backtracking(i + 1);
                currentCombination.RemoveAt(currentCombination.Count - 1);
                currentBlood -= sacrificableSlots[i].blood;
            }
        }
    }
}
