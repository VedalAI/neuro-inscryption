using System.Collections.Generic;
using System.Linq;
using DiskCardGame;

namespace NeuroInscryption.Communication.Json.Actions;

// ReSharper disable once InconsistentNaming
internal sealed class FULL_PlayCardAction : ISchemaModifier
{
    public void Apply(JsonSchema schema)
    {
        if (PlayerHand.Instance.CardsInHand.Count == 0) return;

        JsonSchema cardOptions = new()
        {
            Type = JsonSchemaType.Object,
            Required = ["card", "sacrifices", "lane"],
        };

        foreach (PlayableCard playableCard in PlayerHand.Instance.CardsInHand)
        {
            ApplyCard(playableCard, cardOptions);
        }

        schema.OneOf.Add(new JsonSchema
        {
            Properties = new Dictionary<string, JsonSchema>
            {
                ["action"] = QJS.Const("play_card"),
                ["data"] = cardOptions
            },
        });
    }

    private static void ApplyCard(PlayableCard card, JsonSchema schema)
    {
        if (!card.CanPlay()) return;

        List<int> availableSlots = BoardManager.Instance.playerSlots
            .Select((slot, index) => (slot, index))
            .Where(p => !p.slot.Card)
            .Select(p => p.index)
            .ToList();

        List<(int index, int blood)> sacrificableSlots = BoardManager.Instance.playerSlots
            .Select((slot, index) => (slot, index))
            .Where(p => p.slot.Card && p.slot.Card.CanBeSacrificed)
            .Select(p => (p.index, p.slot.Card.HasAbility(Ability.TripleBlood) ? 3 : 1))
            .ToList();

        if (card.Info.BonesCost > 0 || card.Info.BloodCost == 0)
        {
            schema.OneOf.Add(new JsonSchema
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["card"] = QJS.Const(card.Info.DisplayedNameEnglish), // TODO: unique identifiers for cards
                    ["sacrifices"] = QJS.ConstEmptyArray,
                    ["lane"] = QJS.Enum(availableSlots)
                }
            });
        }
        else
        {
            JsonSchema subschema = new()
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["card"] = QJS.Const(card.Info.DisplayedNameEnglish), // TODO: unique identifiers for cards
                },
                OneOf = []
            };

            foreach (List<int> sacrifice in GetSacrificeCombinations(sacrificableSlots, card.Info.BloodCost))
            {
                subschema.OneOf.Add(new JsonSchema
                {
                    Properties = new Dictionary<string, JsonSchema>
                    {
                        ["sacrifices"] = QJS.Const(sacrifice),
                        ["lane"] = QJS.Enum([..availableSlots, ..sacrifice])
                    }
                });
            }

            schema.OneOf.Add(subschema);
        }
    }

    private static List<List<int>> GetSacrificeCombinations(List<(int index, int blood)> sacrificableSlots, int bloodCost)
    {
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
