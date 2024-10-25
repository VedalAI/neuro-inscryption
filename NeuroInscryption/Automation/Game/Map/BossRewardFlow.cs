using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DiskCardGame;
using HarmonyLib;
using NeuroInscryption.Automation.API;
using NeuroInscryption.Input;
using NeuroInscryption.Utilities.Enumerators;

namespace NeuroInscryption.Automation.Game.Map;

public class BossRewardFlow : HarmonyAutomationFlow<IEnumerator>
{
    public override FlowHook[] Hooks =>
    [
        new()
        {
            Method = AccessTools.EnumeratorMoveNext(typeof(RareCardChoicesSequencer).GetMethod(nameof(RareCardChoicesSequencer.ChooseRareCard))),
            IsValid = FlowHookHelpers.EnumeratorStateIs(8)
        }
    ];
    
    protected override async UniTask ExecuteAsync(IEnumerator target, CancellationToken ct)
    {
        RareCardChoicesSequencer choicer = target.GetThis<RareCardChoicesSequencer>();
        Card card = await Neuro.Cards.ChooseAddToDeck(choicer, ct);
        System.Diagnostics.Debug.Assert(card, "did not pick boss rare");

        await NeuroMouse.MoveToAsync(card, 300, ct);
        await NeuroMouse.ClickAsync(ct);
    }
}