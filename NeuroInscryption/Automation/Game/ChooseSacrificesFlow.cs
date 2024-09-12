using System.Collections;
using System.Reflection;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Json;
using NeuroInscryption.Communication.Json.Actions;

namespace NeuroInscryption.Automation.Game;

public sealed class ChooseSacrificesFlow : AutomationFlow<BoardManager>
{
    public override MethodBase EnableHook => AccessTools.Method(typeof(BoardManager), nameof(BoardManager.ChooseSacrificesForCard));
    public override HarmonyPatchType PatchType => HarmonyPatchType.Prefix;

    protected override IEnumerator Execute(BoardManager target)
    {
        JsonSchema responseSchema = JsonSchema.NewResponseSchema();
        responseSchema.Apply(new ChooseSacrificesAction());

        Decision decision = new()
        {
            Id = "card_battle_turn",
            State = GameState.Board.Current,
            Query = "Choose the next action to take during this card battle.",
            ResponseSchema = responseSchema
        };

    }
}
