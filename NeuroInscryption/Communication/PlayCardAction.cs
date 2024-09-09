using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Communication.Json;

namespace NeuroInscryption.Communication;

internal class PlayCardAction(PlayerHand hand) : ISchemaModifier
{
    public void Apply(JsonSchema schema)
    {
        JsonSchema cardOptions = new()
        {
            Type = JsonSchemaType.Object,
            Required = ["card", "sacrifices", "lane"],
            OneOf = []
        };

        foreach (PlayableCard playableCard in hand.CardsInHand)
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
            Required = ["action", "data"]
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

        if (card.Info.BonesCost > 0)
        {
            schema.OneOf.Add(new JsonSchema
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    ["card"] = QJS.Const(card.Info.DisplayedNameEnglish), // TODO: unique identifiers for cards
                    ["sacrifices"] = QJS.EmptyArray,
                    ["lane"] = QJS.Enum(availableSlots)
                }
            });
        }
        else
        {
            int cost = card.Info.BloodCost;

            foreach (List<int> sacrifice in GetSacrificeCombinations(sacrificableSlots, cost))
            {
                schema.OneOf.Add(new JsonSchema
                {
                    Properties = new Dictionary<string, JsonSchema>
                    {
                        ["card"] = QJS.Const(card.Info.DisplayedNameEnglish), // TODO: unique identifiers for cards
                        ["sacrifices"] = QJS.Array(sacrifice),
                        ["lane"] = QJS.Enum([..availableSlots, ..sacrifice])
                    }
                });
            }
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
