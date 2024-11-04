using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Communication.Data.Serializable;
using NeuroInscryption.Communication.Strings;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public sealed class WoodcarverChooseFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(AccessTools.Method(typeof(BuildTotemSequencer), nameof(BuildTotemSequencer.NewPiecePhase))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(4),
        }
    ];

    protected override async UniTask ExecuteAsync(IEnumerator enumerator, CancellationToken ct)
    {
        BuildTotemSequencer instance = enumerator.GetThis<BuildTotemSequencer>();

        List<TotemPieceData> pieces = instance.slots.Select(slot => TotemPieceData.Create(slot)!).ToList();
        TotemPieceData piece = await Neuro.Loadout.ChooseItem(pieces, DecisionStrings.ChooseTotemPiece, ct);

        await NeuroMouse.MoveAndClick(piece.Slot!, ct);
    }
}
